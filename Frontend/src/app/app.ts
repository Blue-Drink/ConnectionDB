import { CommonModule } from "@angular/common";
import { Component, inject, OnInit } from "@angular/core";
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
public items: Item[] = [];
public filteredItems: Item[] = [];
public searchTerm: string = '';

ngOnInit(): void {
	this.itemService.getItems().subscribe({
		next: (data) => {
			this.items = data;
			this.filteredItems = data;
		},
		error: (err) => {
			console.error('Error la conectar con la API:', err);
		}
	})
}

filterItems() {
	const term = this.searchTerm.toLowerCase();
	this.filteredItems = this.items.filter(item =>
		item.name.toLowerCase().includes(term)
	)
}

title = 'Frontend';

private itemService = inject(ItemService);  

showModal: boolean = false;

newItem: Item = { id: 0, name: '', imgUrl: '', stock: 0 };

saveItem() {
	if (this.isEditing) {
		this.itemService.updateItem(this.newItem.id, this.newItem).subscribe({
			next: () => {
				const index = this.items.findIndex(i => i.id === this.newItem.id);
				this.items[index] = {...this.newItem};
				this.closeModal();
			},
			error: (err) => console.error(err)
		});
	} else {
		this.itemService.postItems(this.newItem).subscribe({
			next: (res) => {
				this.items.push(res);
				this.filterItems();
				this.closeModal();
				console.log('Guardado con éxito');
			},
			error: (err) => console.error('Error al guardar:', err)
		});
	}
}

openModal() { this.showModal = true; }
closeModal() {
	this.showModal = false;
	this.isEditing = false;
	this.newItem = { id: 0, name: '', imgUrl: '', stock: 0 }
}

isEditing: boolean = false;

openEditModal(item: Item) {
	this.isEditing = true;
	this.showModal = true;
	this.newItem = {...item};
}

deleteItem(id: number) {
	if (confirm('¿Está seguro de querer eliminar este artículo?')) {
		this.itemService.deleteItem(id).subscribe({
		next: () => {
			this.items = this.items.filter(i => i.id !== id);
			this.filterItems();
		}
		})
	}
	}
}