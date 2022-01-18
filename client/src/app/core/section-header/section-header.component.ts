import { Component, OnInit } from '@angular/core';
import {BreadcrumbService} from "xng-breadcrumb";
import {Observable} from "rxjs";

@Component({
  selector: 'app-section-header',
  templateUrl: './section-header.component.html',
  styleUrls: ['./section-header.component.scss']
})
export class SectionHeaderComponent implements OnInit {

  // @ts-ignore
  breadcrumb$: Observable<any[]>;
  constructor(private bcService: BreadcrumbService) { }

  ngOnInit() {
    // Configuration options for breadcrumbs
    // breadcrumbs$ is an observable. We're assigning it to a variable, so it can be used in a template
     this.breadcrumb$ = this.bcService.breadcrumbs$;
  }
}
