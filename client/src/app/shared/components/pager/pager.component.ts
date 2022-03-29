import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

@Component({
  selector: 'app-pager',
  templateUrl: './pager.component.html',
  styleUrls: ['./pager.component.scss']
})
export class PagerComponent implements OnInit {

  // Input properties (to be filled wherever the component is used)
  @Input() totalCount!: number;
  @Input() pageSize!: number;
  @Input() pageNumber!: number;

  // Output property - to emit and react on a specific event
  // /i.e., emitting the event from a child to parent component
  @Output() pageChanged = new EventEmitter<number>();

  constructor() { }

  ngOnInit(): void {
  }

  onPagerChange(event: any) {
    this.pageChanged.emit(event.page);
  }
}
