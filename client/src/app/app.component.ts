import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {IProduct} from "./models/product";
import {IPagination} from "./models/pagination";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'E-Commerce';
  // TypeScript 2.7 and up has strict class checking and the property has to be initialized
  // or marked as 'not null' (by adding '!'). Note: "products: any[] = []" will work as well
  products!: IProduct[];

  // Constructor
  constructor(private http: HttpClient) { }

  // Lifecycle methods
  ngOnInit(): void {
    // Gets access to a backend API
    // we need to 'subscribe' to make use of an 'observable'
    this.http.get<IPagination>('https://localhost:5001/api/products?PageSize=20').subscribe(
      (response: IPagination) => {
      this.products = response.data;
    }, error => {
      console.log(error);
    });
  }
}
