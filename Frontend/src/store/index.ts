import { createStore } from 'vuex'
import BrandMoule from '@/store/modules/Brand'
import CategoryModule from '@/store/modules/Category'
import RoleModule from '@/store/modules/Role'

const store = createStore({
  modules: {
    brand: BrandMoule,
    category: CategoryModule,
    role: RoleModule
  },
});

export default store;