import { CommonModule } from "@angular/common";
import { Component, inject, OnInit, signal, computed } from "@angular/core";
import { ItemService } from "./services/item.service";
import { Item } from "./models/item.model";
import { FormsModule } from "@angular/forms";

@Component({
	selector: 'app-root',
	standalone: true,
	imports: [CommonModule, FormsModule],
	templateUrl: './app.html',
	styleUrl: './app.css'
})

export class AppComponent implements OnInit {
	private itemService = inject(ItemService);

	// Inyección y Estados
	public items = signal<Item[]>([]);
	public searchTerm = signal<string>('');
	public isLoading = signal<boolean>(false);
	public errorMessage = signal<string | null>(null);
	private selectedFile: File | null = null;
	public isEditing = false;
	public showModal = false;
	public newItem: Item = { id: 0, name: '', imgRoute: '', stock: 0 };
	public sortOrder = signal<'asc' | 'desc'>('asc');

	// Listado dinámico
	public filteredItems = computed(() => {
		const term = this.removeAccents(this.searchTerm().toLowerCase());
		return this.items().filter(item => {
			const itemName = this.removeAccents(item.name.toLowerCase());
			return itemName.includes(term);
		});
	});

	private removeAccents(str: string): string {
		return str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
	}

	ngOnInit(): void {
		this.loadItems();
	}

	loadItems() {
		this.isLoading.set(true);
		this.errorMessage.set(null);

		this.itemService.getItems({ sort: this.sortOrder() }).subscribe({
			next: (data) => {
				this.items.set(data);
				this.isLoading.set(false);
			},
			error: (err) => {
				this.errorMessage.set("Error al conectar con el servidor.");
				this.isLoading.set(false);
			}
		});
	}

	// Eliminar artículo
	deleteItem(id: number) {
		if (confirm('¿Está seguro de querer eliminar este artículo?')) {
			this.itemService.deleteItem(id).subscribe({
				next: () => {
					this.items.update(prev => prev.filter(i => i.id !== id));
				}
			});
		}
	}

	//


	onFileSelected(event:any) {
		const file: File = event.target.files[0];
		if (file) {
			if (this.newItem.imgRoute && this.newItem.imgRoute.startsWith('blob:')) {
				URL.revokeObjectURL(this.newItem.imgRoute);
			}
			this.selectedFile = file;
			this.newItem.imgRoute = URL.createObjectURL(file);
		}
	}

	// Añadir un artículo
	saveItem() {
		if (!this.newItem.name || (!this.selectedFile && !this.isEditing)){
			this.errorMessage.set("Todos los apartados son obligatorios");
			return;
		}
		this.isLoading.set(true);

		if (this.isEditing) {
			this.itemService.updateItem(this.newItem.id, this.newItem, this.selectedFile || undefined).subscribe({
				next: () => {
					this.loadItems();
					this.closeModal();
				},
				error: (err) => {
					this.errorMessage.set(err.message);
				}
			});
		} else {
			this.itemService.postItems(this.newItem, this.selectedFile || undefined).subscribe({
				next: (res) => {
					this.loadItems();
					this.closeModal();
				},
				error: (err) => {
					this.errorMessage.set(err.message);
				}
			});
		}
		this.isLoading.set(false)
	}

	// Utilidades
	openModal() { this.showModal = true; }

	closeModal() {
		this.showModal = false;
		if (this.newItem.imgRoute?.startsWith('blob:')) {
			URL.revokeObjectURL(this.newItem.imgRoute);
		}
		this.isEditing = false;
		this.selectedFile = null;
		this.newItem = { id: 0, name: '', imgRoute: '', stock: 0 }
	}

	openEditModal(item: Item) {
		this.isEditing = true;
		this.showModal = true;
		this.newItem = {...item};
	}

	toggleSort() {
		this.sortOrder.update(current => current === 'asc' ? 'desc' : 'asc');
		this.loadItems();
	}
}