import {Component, ElementRef, Input, OnInit, Self, ViewChild} from '@angular/core';
import {ControlValueAccessor, NgControl} from "@angular/forms";

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.scss']
})

// ControlValueAccessor defines an interface that acts as a bridge between the Angular forms and native elements in the DOM
export class TextInputComponent implements OnInit, ControlValueAccessor {
  @ViewChild('input', {static: true}) input?: ElementRef;
  @Input() type = 'text';
  @Input() label?: string;

  // In order to access validation we need to access a control as well
  // NgControl is what the Forms controls are derived from
  constructor(@Self() public controlDir: NgControl) {
    this.controlDir.valueAccessor = this;
  }

  ngOnInit() {
    const control = this.controlDir.control;
    const validators = control?.validator ? [control?.validator] : [];
    const asyncValidators = control?.asyncValidator ? [control?.asyncValidator] : [];

    control?.setValidators(validators);
    // @ts-ignore
    control?.setAsyncValidators(asyncValidators);
    control?.updateValueAndValidity();
  }

  // @ts-ignore
  onChange(event) {}
  onTouched() {}

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  writeValue(obj: any): void {
    // @ts-ignore
    this.input.nativeElement.value = obj || '';
  }
}
