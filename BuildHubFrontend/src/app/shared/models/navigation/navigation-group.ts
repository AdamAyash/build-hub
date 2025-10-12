import { LucideIconData } from 'lucide-angular';
import { NavItem } from './navigation-item';

export interface NavGroup {
  id: string;
  label: string;
  icon?: LucideIconData;
  items: NavItem[];
  collapsible?: boolean;
  defaultOpen?: boolean;
}
