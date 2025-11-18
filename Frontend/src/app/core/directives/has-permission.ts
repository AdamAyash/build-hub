import {
  Directive,
  inject,
  Input,
  OnInit,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';

@Directive({
  selector: '[bhHasPermission]',
  standalone: true,
})
export class HasPermission implements OnInit {
  @Input() bhHasPermission!: string | string[];
  @Input() bhHasPermissionMode: 'all' | 'any' = 'all'; // Additional property
  @Input() bhHasPermissionElse?: TemplateRef<HTMLElement>;

  private hasView = false;
  private viewContainer = inject(ViewContainerRef);
  private templateRef = inject(TemplateRef);

  ngOnInit() {
    this.updateView();
  }

  private updateView() {
    const hasPermission = true;

    console.log(this.bhHasPermission);
    console.log(this.bhHasPermissionMode);
    console.log(this.bhHasPermissionElse);

    if (hasPermission && !this.hasView) {
      this.viewContainer.clear();
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (!hasPermission && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }
}
