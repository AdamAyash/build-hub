import { definePreset, palette } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';

export const myPreset = definePreset(Aura, {
  semantic: {
    primary: palette('#030213'),
    colorScheme: {
      light: {},
      dark: {},
    },
  },
  components: {},
});
