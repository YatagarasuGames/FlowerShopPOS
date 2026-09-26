<template>
  <div class="flex flex-col h-screen overflow-hidden bg-zinc-100 dark:bg-zinc-950 text-zinc-900 dark:text-zinc-100">
    <!-- Шапка -->
    <header class="h-16 px-6 bg-white dark:bg-zinc-900 border-b border-zinc-200 dark:border-zinc-800 flex items-center justify-between shrink-0 shadow-xs z-10">
      <div class="flex items-center gap-3">
        <div class="w-10 h-10 rounded-xl bg-emerald-500/10 dark:bg-emerald-500/20 text-emerald-600 dark:text-emerald-400 flex items-center justify-center ring-1 ring-emerald-500/20">
          <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.75" d="M12 21a9.004 9.004 0 008.716-6.747M12 21a9.004 9.004 0 01-8.716-6.747M12 21V12m0 0a3 3 0 10-6 0 3 3 0 006 0zm0 0a3 3 0 106 0 3 3 0 00-6 0zm0 0V3m0 0a3 3 0 10-6 0 3 3 0 006 0zm0 0a3 3 0 106 0 3 3 0 00-6 0z" />
          </svg>
        </div>
        <div>
          <h1 class="font-bold text-sm text-zinc-900 dark:text-zinc-50 leading-tight">Кассовый терминал</h1>
          <p class="text-[11px] text-zinc-400">Сотрудник: <strong class="text-zinc-700 dark:text-zinc-300 font-semibold">{{ authStore.user?.username }}</strong></p>
        </div>
      </div>

      <div class="flex items-center gap-2">
        <button
          @click="openBouquetBuilder"
          class="px-3.5 py-2 rounded-xl bg-pink-50 dark:bg-pink-950/50 border border-pink-200/80 dark:border-pink-900/60 text-pink-600 dark:text-pink-400 hover:bg-pink-100 text-xs font-bold transition flex items-center gap-1.5 cursor-pointer shadow-xs"
        >
          <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
          <span>Собрать букет</span>
        </button>
        <button
          @click="showSupplyModal = true"
          class="px-3.5 py-2 rounded-xl bg-zinc-50 dark:bg-zinc-800 border border-zinc-200 dark:border-zinc-700 hover:bg-zinc-100 text-xs font-medium text-zinc-700 dark:text-zinc-300 transition flex items-center gap-1.5 cursor-pointer"
        >
          <svg class="w-3.5 h-3.5 text-zinc-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
          </svg>
          <span>Приёмка</span>
        </button>
        <button
          @click="showWriteOffModal = true"
          class="px-3.5 py-2 rounded-xl bg-zinc-50 dark:bg-zinc-800 border border-zinc-200 dark:border-zinc-700 hover:bg-zinc-100 text-xs font-medium text-zinc-700 dark:text-zinc-300 transition flex items-center gap-1.5 cursor-pointer"
        >
          <svg class="w-3.5 h-3.5 text-rose-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
          </svg>
          <span>Списание</span>
        </button>

        <div class="h-6 w-px bg-zinc-200 dark:bg-zinc-800 mx-1"></div>

        <button
          @click="themeStore.toggleTheme"
          class="p-2 rounded-xl hover:bg-zinc-100 dark:hover:bg-zinc-800 text-zinc-500 text-sm transition cursor-pointer"
          :title="themeStore.isDark ? 'Светлая тема' : 'Тёмная тема'"
        >
          <svg v-if="themeStore.isDark" class="w-4 h-4 text-amber-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" />
          </svg>
          <svg v-else class="w-4 h-4 text-zinc-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" />
          </svg>
        </button>

        <button
          v-if="authStore.canAccessAdminPanel"
          @click="router.push('/admin')"
          class="px-3.5 py-2 text-xs font-semibold text-emerald-600 dark:text-emerald-400 hover:bg-emerald-50 dark:hover:bg-emerald-950/40 rounded-xl transition cursor-pointer"
        >
          Панель управления
        </button>
        <button
          @click="handleLogout"
          class="px-3.5 py-2 text-xs font-medium text-rose-500 hover:bg-rose-50 dark:hover:bg-rose-950/40 rounded-xl transition cursor-pointer"
        >
          Выйти
        </button>
      </div>
    </header>

    <div class="flex flex-1 overflow-hidden">
      <!-- Каталог витрины -->
      <section class="flex-1 flex flex-col p-5 overflow-hidden">
        <div class="mb-4">
          <div class="relative">
            <span class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-zinc-400">
              <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </span>
            <input
              ref="searchInput"
              v-model="searchQuery"
              type="text"
              placeholder="Поиск по цветам и упаковке (Enter — добавить первый)..."
              class="w-full pl-10 pr-10 py-2.5 rounded-2xl border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 text-zinc-900 dark:text-zinc-100 placeholder-zinc-400 text-xs focus:outline-none focus:ring-2 focus:ring-emerald-500 shadow-xs"
              @keydown.enter="selectFirstFiltered"
            />
            <button v-if="searchQuery" @click="searchQuery = ''" class="absolute inset-y-0 right-0 pr-3.5 flex items-center text-zinc-400 hover:text-zinc-600 cursor-pointer">
              <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        </div>

        <div class="flex-1 overflow-y-auto pr-1">
          <div v-if="filteredProducts.length === 0" class="h-64 flex flex-col items-center justify-center text-zinc-400">
            <svg class="w-12 h-12 mb-2 stroke-1 text-zinc-300 dark:text-zinc-700" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
            </svg>
            <p class="text-xs">Товары не найдены</p>
          </div>

          <!-- Сетка каталога с аппаратной оптимизацией скролла -->
          <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-3">
            <div
              v-for="product in filteredProducts"
              :key="product.id"
              @click="product.stock > 0 && cartStore.addSingleItem(product)"
              :class="[
                'group relative flex flex-col justify-between bg-white dark:bg-zinc-900 border border-zinc-200/80 dark:border-zinc-800/80 rounded-2xl p-3 shadow-xs select-none [content-visibility:auto] [contain-intrinsic-size:160px]',
                product.stock > 0 ? 'hover:border-emerald-500/50 cursor-pointer active:scale-[0.98]' : 'opacity-40 cursor-not-allowed'
              ]"
            >
              <div class="w-full h-28 rounded-xl bg-zinc-100 dark:bg-zinc-800 mb-2 overflow-hidden flex items-center justify-center relative">
                <img
                  v-if="product.imageUrl"
                  :src="apiBaseUrl + product.imageUrl"
                  :alt="product.name"
                  loading="lazy"
                  decoding="async"
                  class="w-full h-full object-cover transition-transform duration-200 group-hover:scale-105"
                />
                <svg v-else class="w-10 h-10 text-zinc-300 dark:text-zinc-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>

                <!-- Бейдж остатка без тяжелого backdrop-blur -->
                <span
                  :class="[
                    'absolute top-2 right-2 px-2 py-0.5 rounded-lg text-[10px] font-bold tracking-wider uppercase shadow-xs',
                    product.stock <= 0 ? 'bg-rose-600 text-white' : product.stock <= 5 ? 'bg-amber-600 text-white' : 'bg-zinc-900/90 text-white'
                  ]"
                >
                  {{ product.stock > 0 ? `${product.stock} шт` : 'Нет' }}
                </span>
              </div>

              <div>
                <h3 class="font-semibold text-xs text-zinc-800 dark:text-zinc-200 line-clamp-2 mb-1">{{ product.name }}</h3>
                <div class="text-xs font-black text-emerald-600 dark:text-emerald-400">
                  <template v-if="product.minPrice && product.maxPrice && product.minPrice !== product.maxPrice">
                    {{ product.minPrice }} – {{ product.maxPrice }} ₽
                  </template>
                  <template v-else>
                    {{ product.minPrice || product.price }} ₽
                  </template>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Чек -->
      <aside class="w-96 bg-white dark:bg-zinc-900 border-l border-zinc-200 dark:border-zinc-800 flex flex-col shrink-0 shadow-lg">
        <div class="p-4 border-b border-zinc-200 dark:border-zinc-800 flex items-center justify-between">
          <div class="flex items-center gap-2">
            <h2 class="font-bold text-sm text-zinc-900 dark:text-zinc-100">Текущий чек</h2>
            <span class="px-2 py-0.5 text-[11px] font-bold rounded-full bg-zinc-100 dark:bg-zinc-800 text-zinc-600 dark:text-zinc-400">
              {{ cartStore.totalItemsCount }}
            </span>
          </div>
          <button
            @click="cartStore.clear"
            :disabled="cartStore.singleItems.length === 0 && cartStore.compositions.length === 0"
            class="text-xs font-semibold text-rose-500 hover:text-rose-600 disabled:opacity-30 cursor-pointer"
          >
            Очистить
          </button>
        </div>

        <div class="flex-1 overflow-y-auto p-4 space-y-3">
          <div v-if="cartStore.singleItems.length === 0 && cartStore.compositions.length === 0" class="h-64 flex flex-col items-center justify-center text-zinc-400 text-center">
            <svg class="w-12 h-12 mb-2 stroke-1 text-zinc-300 dark:text-zinc-700" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
            <p class="text-xs font-medium">Чек пуст</p>
            <p class="text-[11px] text-zinc-400 mt-1">Выберите товары в каталоге или соберите авторский букет</p>
          </div>

          <!-- Композиции -->
          <div
            v-for="comp in cartStore.compositions"
            :key="comp.id"
            class="p-3 rounded-2xl bg-pink-50/50 dark:bg-pink-950/20 border border-pink-200/80 dark:border-pink-900/40 space-y-2"
          >
            <div class="flex items-center justify-between">
              <span class="font-bold text-xs text-pink-700 dark:text-pink-300 flex items-center gap-1.5">
                <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" /></svg>
                {{ comp.name }}
              </span>
              <button @click="cartStore.removeComposition(comp.id)" class="text-zinc-400 hover:text-rose-500 cursor-pointer">
                <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
              </button>
            </div>
            <div v-if="comp.assemblyPrice > 0" class="text-[11px] text-zinc-500 flex justify-between">
              <span>Сборка флориста:</span>
              <span class="font-semibold text-zinc-700 dark:text-zinc-300">{{ comp.assemblyPrice }} ₽</span>
            </div>
            <div class="border-t border-pink-100 dark:border-pink-900/30 pt-1.5 space-y-1">
              <div v-for="cItem in comp.items" :key="cItem.productId" class="text-xs flex justify-between text-zinc-600 dark:text-zinc-400">
                <span>{{ cItem.name }} × {{ cItem.quantity }}</span>
                <span>{{ cartStore.calcItemPrice(cItem) }} ₽</span>
              </div>
            </div>
          </div>

          <!-- Одиночные товары -->
          <div
            v-for="item in cartStore.singleItems"
            :key="item.cartItemId"
            class="p-3 rounded-xl border border-zinc-200/70 dark:border-zinc-800 bg-zinc-50/50 dark:bg-zinc-800/40 flex flex-col gap-1.5"
          >
            <div class="flex items-start justify-between gap-2">
              <div class="min-w-0">
                <h4 class="font-medium text-xs text-zinc-900 dark:text-zinc-100 truncate">{{ item.name }}</h4>
                <div class="text-[11px] text-zinc-400 mt-0.5 flex items-center gap-1.5">
                  <span>Партия:</span>
                  <span :class="{ 'line-through opacity-50': item.discountType !== 0 }" class="font-semibold">{{ item.sellingPrice }} ₽</span>
                  <span v-if="item.discountType !== 0" class="font-bold text-rose-500">
                    {{ cartStore.getDiscountedPrice(item.sellingPrice, item.discountType, item.discountValue) }} ₽
                  </span>
                  <span>/ шт</span>
                </div>
              </div>

              <span class="font-black text-xs text-zinc-900 dark:text-zinc-100">
                {{ cartStore.calcItemPrice(item) }} ₽
              </span>
            </div>

            <div class="flex items-center justify-between pt-1 border-t border-zinc-200/50 dark:border-zinc-700/50">
              <div class="inline-flex items-center bg-white dark:bg-zinc-800 border border-zinc-200 dark:border-zinc-700 rounded-lg overflow-hidden shadow-2xs">
                <button @click="cartStore.updateSingleQuantity(item.cartItemId, -1)" class="w-6 h-6 hover:bg-zinc-100 dark:hover:bg-zinc-700 font-bold text-xs text-zinc-600 dark:text-zinc-300 cursor-pointer">-</button>
                <span class="w-7 text-center font-bold text-xs text-zinc-800 dark:text-zinc-200">{{ item.quantity }}</span>
                <button @click="cartStore.updateSingleQuantity(item.cartItemId, 1)" :disabled="item.quantity >= item.maxStock" class="w-6 h-6 hover:bg-zinc-100 dark:hover:bg-zinc-700 font-bold text-xs text-zinc-600 dark:text-zinc-300 disabled:opacity-30 cursor-pointer">+</button>
              </div>

              <button
                @click="openDiscountModal(item)"
                :class="[
                  'px-2 py-1 rounded-md text-[10px] font-bold border transition cursor-pointer',
                  item.discountType !== 0 ? 'bg-amber-100 text-amber-800 border-amber-300 dark:bg-amber-950 dark:text-amber-300 dark:border-amber-800' : 'bg-white dark:bg-zinc-800 border-zinc-200 dark:border-zinc-700 text-zinc-500'
                ]"
              >
                {{ item.discountType === 1 ? `-${item.discountValue}%` : item.discountType === 2 ? `${item.discountValue}₽` : 'Скидка на партию' }}
              </button>

              <button @click="cartStore.removeSingleItem(item.cartItemId)" class="text-zinc-400 hover:text-rose-500 cursor-pointer">
                <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
              </button>
            </div>
          </div>
        </div>

        <div class="p-4 bg-zinc-50 dark:bg-zinc-900/90 border-t border-zinc-200 dark:border-zinc-800 shrink-0 space-y-3">
          <div class="flex items-baseline justify-between">
            <span class="text-xs font-bold uppercase text-zinc-400 tracking-wider">К оплате</span>
            <span class="text-2xl font-black text-emerald-600 dark:text-emerald-400">
              {{ cartStore.totalAmount.toLocaleString() }} ₽
            </span>
          </div>

          <button
            @click="handleCheckout"
            :disabled="(cartStore.singleItems.length === 0 && cartStore.compositions.length === 0) || isProcessing"
            class="w-full py-3.5 rounded-xl bg-emerald-600 hover:bg-emerald-700 active:scale-[0.99] disabled:opacity-40 text-white font-bold text-xs shadow-lg shadow-emerald-600/20 transition-all flex items-center justify-center gap-2 cursor-pointer"
          >
            <span v-if="isProcessing" class="animate-spin inline-block w-4 h-4 border-2 border-white border-t-transparent rounded-full"></span>
            <span>{{ isProcessing ? 'Пробитие чека...' : 'Оплатить чек' }}</span>
          </button>
        </div>
      </aside>
    </div>

    <!-- Модалка: Конструктор букета -->
    <div v-if="showBouquetModal" class="fixed inset-0 z-50 bg-black/60 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 rounded-3xl w-full max-w-2xl overflow-hidden shadow-2xl flex flex-col max-h-[85vh]">
        <div class="p-5 border-b border-zinc-200 dark:border-zinc-800 flex justify-between items-center">
          <h3 class="font-bold text-sm text-zinc-900 dark:text-zinc-100 flex items-center gap-2">
            <svg class="w-4 h-4 text-pink-500" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" /></svg>
            Конструктор букета / композиции
          </h3>
          <button @click="showBouquetModal = false" class="text-zinc-400 hover:text-zinc-600 cursor-pointer">✕</button>
        </div>

        <div class="p-5 flex-1 overflow-y-auto space-y-4">
          <div class="grid grid-cols-3 gap-3">
            <div class="col-span-2">
              <label class="block text-xs font-semibold text-zinc-600 dark:text-zinc-400 mb-1">Название композиции</label>
              <input v-model="bouquetForm.name" type="text" class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800 focus:outline-none focus:ring-1 focus:ring-pink-500" />
            </div>
            <div>
              <label class="block text-xs font-semibold text-zinc-600 dark:text-zinc-400 mb-1">Сборка флориста (₽)</label>
              <input v-model.number="bouquetForm.assemblyPrice" type="number" min="0" step="50" class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800 focus:outline-none focus:ring-1 focus:ring-pink-500" />
            </div>
          </div>

          <div class="grid grid-cols-2 gap-4 h-72">
            <div class="border border-zinc-200 dark:border-zinc-800 rounded-xl p-3 flex flex-col">
              <input v-model="bouquetSearch" type="text" placeholder="Поиск цветов..." class="w-full px-3 py-1.5 text-xs rounded-lg border border-zinc-200 dark:border-zinc-700 mb-2 bg-white dark:bg-zinc-800 focus:outline-none" />
              <div class="flex-1 overflow-y-auto space-y-1 pr-1">
                <div
                  v-for="p in availableBouquetProducts"
                  :key="p.id"
                  @click="addFlowerToBouquet(p)"
                  class="p-2 rounded-lg hover:bg-zinc-100 dark:hover:bg-zinc-800 flex justify-between items-center text-xs cursor-pointer"
                >
                  <span class="font-medium truncate">{{ p.name }}</span>
                  <div class="text-right shrink-0">
                    <span class="font-bold text-emerald-600">{{ p.minPrice || p.price }} ₽</span>
                    <span class="text-[10px] text-zinc-400 block">{{ p.stock }} шт</span>
                  </div>
                </div>
              </div>
            </div>

            <div class="border border-zinc-200 dark:border-zinc-800 rounded-xl p-3 flex flex-col bg-zinc-50/50 dark:bg-zinc-800/20">
              <span class="text-xs font-semibold text-zinc-500 mb-2">Состав букета:</span>
              <div v-if="bouquetForm.items.length === 0" class="flex-1 flex items-center justify-center text-zinc-400 text-xs text-center">
                Выберите цветы из списка слева
              </div>
              <div v-else class="flex-1 overflow-y-auto space-y-2 pr-1">
                <div v-for="bItem in bouquetForm.items" :key="bItem.productId" class="flex items-center justify-between text-xs bg-white dark:bg-zinc-800 p-2 rounded-lg shadow-2xs">
                  <span class="truncate mr-2 font-medium">{{ bItem.name }}</span>
                  <div class="flex items-center gap-2 shrink-0">
                    <div class="flex items-center border border-zinc-200 dark:border-zinc-700 rounded-md">
                      <button @click="bItem.quantity > 1 ? bItem.quantity-- : removeFlowerFromBouquet(bItem.productId)" class="w-5 h-5 font-bold cursor-pointer">-</button>
                      <span class="w-5 text-center font-bold">{{ bItem.quantity }}</span>
                      <button @click="bItem.quantity < bItem.stock && bItem.quantity++" class="w-5 h-5 font-bold cursor-pointer">+</button>
                    </div>
                    <span class="font-bold w-12 text-right">{{ bItem.sellingPrice * bItem.quantity }} ₽</span>
                    <button @click="removeFlowerFromBouquet(bItem.productId)" class="text-rose-500 cursor-pointer">✕</button>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="p-3 bg-pink-50 dark:bg-pink-950/40 rounded-xl flex justify-between items-center text-xs">
            <span class="font-semibold text-pink-800 dark:text-pink-300">Стоимость букета со сборкой:</span>
            <strong class="text-base font-black text-pink-600 dark:text-pink-400">{{ calculatedBouquetTotal }} ₽</strong>
          </div>
        </div>

        <div class="p-4 bg-zinc-50 dark:bg-zinc-900 border-t border-zinc-200 dark:border-zinc-800 flex justify-end gap-2">
          <button @click="showBouquetModal = false" class="px-4 py-2 text-xs font-semibold text-zinc-600 rounded-xl cursor-pointer">Отмена</button>
          <button @click="submitBouquetToCart" :disabled="bouquetForm.items.length === 0" class="px-5 py-2 text-xs font-bold text-white bg-pink-600 hover:bg-pink-700 disabled:opacity-40 rounded-xl cursor-pointer">В чек</button>
        </div>
      </div>
    </div>

    <!-- Модалка: Скидка на партию -->
    <div v-if="editingDiscountItem" class="fixed inset-0 z-50 bg-black/60 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 rounded-3xl w-full max-w-sm p-6 shadow-2xl space-y-4">
        <div>
          <h3 class="font-bold text-sm text-zinc-900 dark:text-zinc-100 mb-1">Скидка / Уценка партии</h3>
          <p class="text-xs text-zinc-400">{{ editingDiscountItem.name }} (Текущая цена: {{ editingDiscountItem.sellingPrice }} ₽)</p>
        </div>

        <div class="grid grid-cols-3 gap-2">
          <button
            v-for="mode in [{ id: 0, label: 'Без скидки' }, { id: 1, label: '% Процент' }, { id: 2, label: '₽ Фикс цена' }]"
            :key="mode.id"
            @click="discountModalData.type = mode.id"
            :class="[
              'py-2 px-1 text-xs font-bold rounded-xl border text-center transition cursor-pointer',
              discountModalData.type === mode.id ? 'bg-amber-500 border-amber-600 text-white' : 'border-zinc-200 dark:border-zinc-700 text-zinc-600 dark:text-zinc-400'
            ]"
          >
            {{ mode.label }}
          </button>
        </div>

        <div v-if="discountModalData.type === 1">
          <label class="block text-xs font-semibold text-zinc-500 mb-1">Скидка в процентах (%)</label>
          <input v-model.number="discountModalData.value" type="number" min="1" max="99" class="w-full px-3 py-2 rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800 text-xs font-bold" />
          <p class="text-[11px] text-zinc-400 mt-1">Новая цена: {{ Math.round(editingDiscountItem.sellingPrice * (1 - (discountModalData.value || 0)/100)) }} ₽</p>
        </div>

        <div v-if="discountModalData.type === 2">
          <label class="block text-xs font-semibold text-zinc-500 mb-1">Новая цена за 1 шт. (₽)</label>
          <input v-model.number="discountModalData.value" type="number" min="0" class="w-full px-3 py-2 rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800 text-xs font-bold" />
        </div>

        <div class="flex justify-end gap-2 pt-2">
          <button @click="editingDiscountItem = null" class="px-4 py-2 text-xs font-semibold text-zinc-600 rounded-xl cursor-pointer">Отмена</button>
          <button @click="applyDiscount" class="px-5 py-2 text-xs font-bold text-white bg-emerald-600 hover:bg-emerald-700 rounded-xl cursor-pointer">Применить</button>
        </div>
      </div>
    </div>

    <!-- Модалка: Приём поставки -->
    <div v-if="showSupplyModal" class="fixed inset-0 z-50 bg-black/60 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 rounded-3xl w-full max-w-md p-6 shadow-2xl space-y-4">
        <h3 class="font-bold text-sm text-zinc-900 dark:text-zinc-100 flex items-center gap-2">
          <svg class="w-4 h-4 text-emerald-500" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" /></svg>
          Приёмка новой партии товара
        </h3>
        <form @submit.prevent="handleQuickSupply" class="space-y-3">
          <div>
            <label class="block text-xs font-semibold text-zinc-500 mb-1">Товар</label>
            <select v-model="quickSupply.productId" required class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800">
              <option value="" disabled>-- Выберите позицию --</option>
              <option v-for="p in products" :key="p.id" :value="p.id">{{ p.name }} (На складе: {{ p.stock }} шт)</option>
            </select>
          </div>
          <div>
            <label class="block text-xs font-semibold text-zinc-500 mb-1">Количество поступивших (шт)</label>
            <input v-model.number="quickSupply.quantity" type="number" min="1" required class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800" />
          </div>
          <div class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-xs font-semibold text-zinc-500 mb-1">Закупка 1 шт (₽)</label>
              <input v-model.number="quickSupply.purchasePrice" type="number" step="0.5" min="0" required class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800" />
            </div>
            <div>
              <label class="block text-xs font-semibold text-zinc-500 mb-1">Розница 1 шт (₽)</label>
              <input v-model.number="quickSupply.sellingPrice" type="number" step="0.5" min="0" required class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800" />
            </div>
          </div>
          <div class="p-3 bg-zinc-100 dark:bg-zinc-800 rounded-xl text-xs flex justify-between">
            <span>Себестоимость партии:</span>
            <strong>{{ (quickSupply.quantity * quickSupply.purchasePrice).toLocaleString() }} ₽</strong>
          </div>
          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showSupplyModal = false" class="px-4 py-2 text-xs font-semibold text-zinc-600 rounded-xl cursor-pointer">Отмена</button>
            <button type="submit" :disabled="isSubmittingModal" class="px-5 py-2 text-xs font-bold text-white bg-emerald-600 hover:bg-emerald-700 rounded-xl cursor-pointer">Оприходовать</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Модалка: Списание цветов -->
    <div v-if="showWriteOffModal" class="fixed inset-0 z-50 bg-black/60 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 rounded-3xl w-full max-w-md p-6 shadow-2xl space-y-4">
        <h3 class="font-bold text-sm text-zinc-900 dark:text-zinc-100 flex items-center gap-2">
          <svg class="w-4 h-4 text-rose-500" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
          Списание испорченных цветов
        </h3>
        <form @submit.prevent="handleQuickWriteOff" class="space-y-3">
          <div>
            <label class="block text-xs font-semibold text-zinc-500 mb-1">Товар</label>
            <select v-model="quickWriteOff.productId" required class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800">
              <option value="" disabled>-- Выберите товар --</option>
              <option v-for="p in products.filter(x => x.stock > 0)" :key="p.id" :value="p.id">{{ p.name }} (Доступно: {{ p.stock }} шт)</option>
            </select>
          </div>
          <div>
            <label class="block text-xs font-semibold text-zinc-500 mb-1">Количество (шт)</label>
            <input v-model.number="quickWriteOff.quantity" type="number" min="1" :max="selectedProductMaxStock" required class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800" />
          </div>
          <div>
            <label class="block text-xs font-semibold text-zinc-500 mb-1">Причина списания</label>
            <div class="flex flex-wrap gap-1.5 mb-2">
              <button
                type="button"
                v-for="preset in ['Увядание / несвежий', 'Сломан стебель', 'Брак поставки']"
                :key="preset"
                @click="quickWriteOff.reason = preset"
                :class="['px-2.5 py-1 text-[11px] rounded-full border transition cursor-pointer', quickWriteOff.reason === preset ? 'bg-rose-50 border-rose-300 text-rose-700 dark:bg-rose-950 dark:border-rose-800 dark:text-rose-300 font-bold' : 'border-zinc-200 dark:border-zinc-700 text-zinc-600']"
              >
                {{ preset }}
              </button>
            </div>
            <input v-model="quickWriteOff.reason" type="text" required placeholder="Или укажите свою причину..." class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800" />
          </div>
          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showWriteOffModal = false" class="px-4 py-2 text-xs font-semibold text-zinc-600 rounded-xl cursor-pointer">Отмена</button>
            <button type="submit" :disabled="isSubmittingModal || !isWriteOffValid" class="px-5 py-2 text-xs font-bold text-white bg-rose-600 hover:bg-rose-700 disabled:opacity-40 rounded-xl cursor-pointer">Списать</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useCartStore } from '../stores/cart'
import { useThemeStore } from '../stores/theme'
import apiClient from '../api/axios'

const router = useRouter()
const authStore = useAuthStore()
const cartStore = useCartStore()
const themeStore = useThemeStore()

const apiBaseUrl = import.meta.env.VITE_UPLOADS_URL || ''
const products = ref([])
const searchQuery = ref('')
const isProcessing = ref(false)
const isSubmittingModal = ref(false)
const searchInput = ref(null)

const showSupplyModal = ref(false)
const showWriteOffModal = ref(false)
const showBouquetModal = ref(false)

const quickSupply = ref({ productId: '', quantity: 25, purchasePrice: 120, sellingPrice: 200 })
const quickWriteOff = ref({ productId: '', quantity: 1, reason: 'Увядание / несвежий' })

const bouquetSearch = ref('')
const bouquetForm = ref({ name: 'Сборный букет', assemblyPrice: 200, items: [] })

const openBouquetBuilder = () => {
  bouquetForm.value = { name: 'Сборный букет', assemblyPrice: 200, items: [] }
  bouquetSearch.value = ''
  showBouquetModal.value = true
}

const availableBouquetProducts = computed(() => {
  const q = bouquetSearch.value.toLowerCase().trim()
  return products.value.filter(p => p.stock > 0 && (!q || p.name.toLowerCase().includes(q)))
})

const addFlowerToBouquet = (product) => {
  const existing = bouquetForm.value.items.find(i => i.productId === product.id)
  if (existing) {
    if (existing.quantity < product.stock) existing.quantity++
  } else {
    bouquetForm.value.items.push({
      productId: product.id,
      name: product.name,
      sellingPrice: product.minPrice || product.price,
      stock: product.stock,
      quantity: 1,
      discountType: 0,
      discountValue: 0
    })
  }
}

const removeFlowerFromBouquet = (productId) => {
  bouquetForm.value.items = bouquetForm.value.items.filter(i => i.productId !== productId)
}

const calculatedBouquetTotal = computed(() => {
  const itemsTotal = bouquetForm.value.items.reduce((s, i) => s + cartStore.calcItemPrice(i), 0)
  return itemsTotal + (Number(bouquetForm.value.assemblyPrice) || 0)
})

const submitBouquetToCart = () => {
  if (bouquetForm.value.items.length === 0) return
  cartStore.addComposition(bouquetForm.value)
  showBouquetModal.value = false
}

// Уценка
const editingDiscountItem = ref(null)
const discountModalData = ref({ type: 0, value: 0 })

const openDiscountModal = (item) => {
  editingDiscountItem.value = item
  discountModalData.value = { type: item.discountType, value: item.discountValue || 0 }
}

const applyDiscount = () => {
  if (editingDiscountItem.value) {
    cartStore.setSingleItemDiscount(editingDiscountItem.value.cartItemId, discountModalData.value.type, discountModalData.value.value)
  }
  editingDiscountItem.value = null
}

const loadProducts = async () => {
  try {
    const res = await apiClient.get('/products?onlyActive=true')
    products.value = res.data
  } catch (err) {
    console.error('Ошибка загрузки товаров', err)
  }
}

onMounted(() => {
  loadProducts()
  searchInput.value?.focus()
})

const filteredProducts = computed(() => {
  if (!searchQuery.value.trim()) return products.value
  const q = searchQuery.value.toLowerCase()
  return products.value.filter(p => p.name.toLowerCase().includes(q))
})

const selectFirstFiltered = () => {
  if (filteredProducts.value.length > 0 && filteredProducts.value[0].stock > 0) {
    cartStore.addSingleItem(filteredProducts.value[0])
    searchQuery.value = ''
  }
}

const handleCheckout = async () => {
  try {
    isProcessing.value = true
    await cartStore.checkout()
    await loadProducts()
    searchInput.value?.focus()
  } catch (err) {
    alert(err.response?.data?.message || 'Ошибка при пробитии чека')
  } finally {
    isProcessing.value = false
  }
}

const handleQuickSupply = async () => {
  try {
    isSubmittingModal.value = true
    await apiClient.post('/supplies', quickSupply.value)
    alert('Поставка оприходована!')
    showSupplyModal.value = false
    quickSupply.value = { productId: '', quantity: 25, purchasePrice: 120, sellingPrice: 200 }
    await loadProducts()
  } catch (err) {
    alert(err.response?.data?.message || 'Ошибка оприходования')
  } finally {
    isSubmittingModal.value = false
  }
}

const selectedProductMaxStock = computed(() => {
  if (!quickWriteOff.value.productId) return 1
  const p = products.value.find(x => x.id === quickWriteOff.value.productId)
  return p ? p.stock : 1
})

const isWriteOffValid = computed(() => {
  if (!quickWriteOff.value.productId || quickWriteOff.value.quantity <= 0) return false
  return quickWriteOff.value.quantity <= selectedProductMaxStock.value && !!quickWriteOff.value.reason.trim()
})

const handleQuickWriteOff = async () => {
  try {
    isSubmittingModal.value = true
    await apiClient.post('/writeoffs', {
      productId: quickWriteOff.value.productId,
      quantity: quickWriteOff.value.quantity,
      reason: quickWriteOff.value.reason.trim()
    })
    alert('Списание зафиксировано!')
    showWriteOffModal.value = false
    quickWriteOff.value = { productId: '', quantity: 1, reason: 'Увядание / несвежий' }
    await loadProducts()
  } catch (err) {
    alert(err.response?.data?.message || 'Ошибка списания')
  } finally {
    isSubmittingModal.value = false
  }
}

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}
</script>
