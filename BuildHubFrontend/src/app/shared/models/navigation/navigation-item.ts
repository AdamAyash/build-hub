import { LucideIconData } from "lucide-angular";
import { Page } from "./pages";

export interface NavItem {
  id: string;
  label: string;
  icon: LucideIconData;
  page: Page;
  badge?: {
    content: string | number;
    variant?: 'default' | 'secondary' | 'destructive' | 'outline';
    pulse?: boolean;
  };
  description?: string;
  shortcut?: string;
  isNew?: boolean;
}
