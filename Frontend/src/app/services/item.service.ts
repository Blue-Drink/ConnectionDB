import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Item } from "../models/item.model";
import { catchError, throwError } from "rxjs";

@Injectable({
	providedIn: 'root'
})

export class ItemService {
	private apiUrl = 'https://localhost:7221/api/items';

	constructor(private http: HttpClient){}

	getItems(): Observable<Item[]> {
		return this.http.get<Item[]>(this.apiUrl).pipe(
			catchError(error => {
				console.error('Error en la petición:', error);
				return throwError(() => new Error('No se pudo obtener la lista de artículos.'))
			})
		);
	}

	postItems(item: Item): Observable<Item> {
		return this.http.post<Item>(this.apiUrl, item).pipe(
			catchError(error => {
				console.error('Error en la petición', error);
				return throwError(() => new Error('No se pudo añadir el artículo.'))
			})
		);
	}

	updateItem(id: number, item: Item): Observable<any> {
		return this.http.put(`${this.apiUrl}/${id}`, item).pipe(
			catchError(error => {
				console.error('Error en la petición', error);
				return throwError(() => new Error('No se pudo modificar el artículo.'))
			})
		);
	}

	deleteItem(id: number): Observable<any> {
		return this.http.delete(`${this.apiUrl}/${id}`).pipe(
			catchError(error => {
				console.error('Error en la petición', error);
				return throwError(() => new Error('No se pudo eliminar el artículo.'))
			})
		);
	}
}