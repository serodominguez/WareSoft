import { createStore } from 'vuex'
import Category from '@/store/modules/Category'

const store = createStore({
  modules: {
    Category,
  },
});

export default store;