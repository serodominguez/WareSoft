import { createStore } from 'vuex'
import CategoryModule from '@/store/modules/Category'

const store = createStore({
  modules: {
    category: CategoryModule,
  },
});

export default store;