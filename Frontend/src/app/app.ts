import { CommonModule } from "@angular/common";
import { Component, inject, OnInit } from "@angular/core";
import { ItemService } from "./services/item.service";
import { Item } from "./models/item.model";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})

export class AppComponent implements OnInit {
  title = 'Frontend';

  private itemService = inject(ItemService);

  public items: Item[] = [];

  ngOnInit(): void {
    this.itemService.getItems().subscribe({
      next: (data) => {
        this.items = data;
        console.log('Datos recibidos:', this.items);
      },
      error: (err) => {
        console.error('Error la conectar con la API:', err);
      }
    })
  }
}