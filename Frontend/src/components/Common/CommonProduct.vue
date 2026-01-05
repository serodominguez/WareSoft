<template>
  <v-dialog max-width="1200px" persistent>
    <v-card elevation="2">
      <v-card-title class="bg-surface-light pt-4">
        <span>Seleccione el Producto</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-row justify="center">
          <v-col cols="12" md="8" lg="8" xl="8" class="mb-2">
            <v-text-field append-inner-icon="search" density="compact"
              label="Ingrese el código, precio, categoría o la marca" variant="solo" hide-details single-line
              @click:append-inner="" class="elevation-1"></v-text-field>
          </v-col>
        </v-row>
        <v-data-table :items-per-page-options="[5, 10, 20]" :items-per-page="5"
          loading-text="Cargando... Espere por favor" page-text="{0}-{1} de {2}">
          <template v-slot:item="{ item }">
            <tr>
              <td>{{ item.code }}</td>
              <td>{{ item.description }}</td>
              <td>{{ item.material }}</td>
              <td>{{ item.colour }}</td>
              <td>{{ item.price }}</td>
              <td>{{ item.categorY_NAME }}</td>
              <td>{{ item.branD_NAME }}</td>
              <td>
                <v-btn color="blue" icon="add" variant="text" @click="" size="small"></v-btn>
              </td>
            </tr>
          </template>
        </v-data-table>
      </v-card-text>
      <v-col>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="red" dark class="mb-2" elevation="4" @click="">Cerrar</v-btn>
        </v-card-actions>
      </v-col>
    </v-card>
  </v-dialog>
</template>