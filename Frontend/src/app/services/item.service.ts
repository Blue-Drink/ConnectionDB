import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Item } from "../models/item.model";

@Injectable({
     providedIn: 'root'
})

export class ItemService {
     private apiUrl = 'https://localhost:7221/api/items';

     constructor(private http: HttpClient){}

     getItems(): Observable<Item[]> {
          return this.http.get<Item[]>(this.apiUrl);
     }

     postItems(item: Item): Observable<Item> {
          return this.http.post<Item>(this.apiUrl, item);
     }

     updateItem(id: number, item: Item): Observable<any> {
          return this.http.put(`${this.apiUrl}/${id}`, item);
     }

     deleteItem(id: number): Observable<any> {
          return this.http.delete(`${this.apiUrl}/${id}`);
     }
}