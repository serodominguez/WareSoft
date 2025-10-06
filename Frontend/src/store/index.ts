import { createStore } from 'vuex'
import BrandMoule from '@/store/modules/Brand'
import CategoryModule from '@/store/modules/Category'

const store = createStore({
  modules: {
    category: CategoryModule,
    brand: BrandMoule
  },
});

export default store;