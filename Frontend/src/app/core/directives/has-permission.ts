import {
  Directive,
  inject,
  OnInit,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';

@Directive({
  selector: '[bhHasPermission]',
  standalone: true,
})
export class HasPermission implements OnInit {
  private hasView = false;
  private viewContainer = inject(ViewContainerRef);
  private templateRef = inject(TemplateRef);

  ngOnInit() {
    this.updateView();
  }

  private updateView() {
    const hasPermission = true;

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
