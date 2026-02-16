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

	public isEditing = false;
	public showModal = false;
	public newItem: Item = { id: 0, name: '', imgUrl: '', stock: 0 };

	// Listado dinámico
	public filteredItems = computed(() => {
		const term = this.searchTerm().toLowerCase();
		return this.items().filter(item =>
			item.name.toLowerCase().includes(term)
		);
	});

	ngOnInit(): void {
		this.loadItems();
	}

	loadItems() {
		this.isLoading.set(true);
		this.errorMessage.set(null);

		this.itemService.getItems().subscribe({
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

	// Añadir un artículo
	saveItem() {
		if (!this.newItem.name) return;
		this.isLoading.set(true);

		if (this.isEditing) {
			this.itemService.updateItem(this.newItem.id, this.newItem).subscribe({
				next: () => {
					this.items.update(prev =>
						prev.map(i => i.id === this.newItem.id ? {...this.newItem} : i)
					);
					this.closeModal();
				},
				error: (err) => {
					this.errorMessage.set(err.message);
				}
			});
		} else {
			this.itemService.postItems(this.newItem).subscribe({
				next: (res) => {
					this.items.update(prev => [...prev, res]);
					this.closeModal();;
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
		this.isEditing = false;
		this.newItem = { id: 0, name: '', imgUrl: '', stock: 0 }
	}

	openEditModal(item: Item) {
		this.isEditing = true;
		this.showModal = true;
		this.newItem = {...item};
	}
}