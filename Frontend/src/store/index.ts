import { createStore } from 'vuex'
import BrandMoule from '@/store/modules/Brand'
import CategoryModule from '@/store/modules/Category'
import RoleModule from '@/store/modules/Role'
import StoreModule from '@/store/modules/Store'
import UserModule from '@/store/modules/User'

const store = createStore({
  modules: {
    brand: BrandMoule,
    category: CategoryModule,
    role: RoleModule,
    store: StoreModule,
    user: UserModule
  },
});

export default store;