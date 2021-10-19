import {Component, OnInit} from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'E-Commerce';
  // TypeScript 2.7 and up has strict class checking and the property has to be initialized
  // or marked as 'not null' (by adding '!'). Note: "products: any[] = []" will work as well
  //products!: IProduct[];

  // Constructor
  constructor() { }

  // Lifecycle methods
  ngOnInit(): void {

  }
}
