import { HttpClient, HttpParams } from "@angular/common/http";
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

	getItems(params?: {sort?: string}): Observable<Item[]> {
		let httpParams = new HttpParams();

		if (params?.sort) {
			httpParams = httpParams.set('sort', params.sort);
		}

		return this.http.get<Item[]>(this.apiUrl, {params: httpParams}).pipe(
			catchError(error => {
				console.error('Error en la petición:', error);
				return throwError(() => new Error('No se pudo obtener la lista de artículos.'))
			})
		);
	}

	private createFormData(item: Item, file?: File): FormData{
		const formData = new FormData();
		formData.append('name', item.name);
		formData.append('stock', item.stock.toString());
		if (file) {
			formData.append('imageFile', file);
		}
		return formData;
	}

	postItems(item: Item, file?: File): Observable<Item> {
		const formData = new FormData();
		formData.append('name', item.name);
		formData.append('stock', item.stock.toString());

		if (file) {
			formData.append('imageFile', file);
		}

		return this.http.post<Item>(this.apiUrl, formData).pipe(
			catchError(error => {
				console.error('Error en la petición', error);
				return throwError(() => new Error('No se pudo añadir el artículo.'))
			})
		);
	}

	updateItem(id: number, item: Item, file?: File): Observable<any> {
		const data = this.createFormData(item, file);
		return this.http.put(`${this.apiUrl}/${id}`, data).pipe(
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