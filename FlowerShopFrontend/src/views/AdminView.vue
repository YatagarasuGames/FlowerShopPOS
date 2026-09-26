<template>
  <div class="min-h-screen bg-zinc-100 dark:bg-zinc-950 flex flex-col text-zinc-900 dark:text-zinc-100 select-none">
    <!-- Шапка панели управления -->
    <header class="h-16 px-6 bg-white dark:bg-zinc-900 border-b border-zinc-200 dark:border-zinc-800 flex items-center justify-between shadow-xs shrink-0">
      <div class="flex items-center gap-3">
        <div class="w-10 h-10 rounded-xl bg-emerald-500/10 dark:bg-emerald-500/20 text-emerald-600 dark:text-emerald-400 flex items-center justify-center ring-1 ring-emerald-500/20">
          <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.75" d="M12 21a9.004 9.004 0 008.716-6.747M12 21a9.004 9.004 0 01-8.716-6.747M12 21V12m0 0a3 3 0 10-6 0 3 3 0 006 0zm0 0a3 3 0 106 0 3 3 0 00-6 0zm0 0V3m0 0a3 3 0 10-6 0 3 3 0 006 0zm0 0a3 3 0 106 0 3 3 0 00-6 0z" />
          </svg>
        </div>
        <div>
          <h1 class="font-bold text-sm text-zinc-900 dark:text-zinc-50 leading-tight">Панель управления</h1>
          <p class="text-[11px] text-zinc-400">Цветочный склад и управление продажами</p>
        </div>
        
        <span
          :class="[
            'ml-2 px-2.5 py-0.5 text-[10px] font-bold tracking-wider uppercase rounded-full border',
            authStore.isAdmin 
              ? 'bg-rose-50 dark:bg-rose-950/60 text-rose-600 dark:text-rose-400 border-rose-200 dark:border-rose-800' 
              : authStore.isOwner 
                ? 'bg-emerald-50 dark:bg-emerald-950/60 text-emerald-600 dark:text-emerald-400 border-emerald-200 dark:border-emerald-800'
                : 'bg-blue-50 dark:bg-blue-950/60 text-blue-600 dark:text-blue-400 border-blue-200 dark:border-blue-800'
          ]"
        >
          {{ authStore.isAdmin ? 'Администратор' : authStore.isOwner ? 'Владелец' : 'Кассир' }}
        </span>
      </div>

      <div class="flex items-center gap-2.5">
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
          @click="router.push('/pos')"
          class="px-3.5 py-2 text-xs font-semibold rounded-xl bg-zinc-100 dark:bg-zinc-800 hover:bg-zinc-200 dark:hover:bg-zinc-700 transition cursor-pointer flex items-center gap-1.5"
        >
          <svg class="w-3.5 h-3.5 text-zinc-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
          </svg>
          <span>Терминал</span>
        </button>

        <button
          @click="handleLogout"
          class="px-3.5 py-2 text-xs font-semibold text-rose-500 hover:bg-rose-50 dark:hover:bg-rose-950/40 rounded-xl transition cursor-pointer"
        >
          Выйти
        </button>
      </div>
    </header>

    <!-- Вкладки навигации -->
    <div class="px-6 bg-white dark:bg-zinc-900 border-b border-zinc-200 dark:border-zinc-800 flex gap-1 overflow-x-auto shrink-0">
      <button
        v-for="tab in visibleTabs"
        :key="tab.id"
        @click="switchTab(tab.id)"
        :class="[
          'py-3 px-4 text-xs font-bold border-b-2 transition cursor-pointer whitespace-nowrap flex items-center gap-2',
          activeTab === tab.id 
            ? 'border-emerald-600 text-emerald-600 dark:text-emerald-400 dark:border-emerald-400' 
            : 'border-transparent text-zinc-500 hover:text-zinc-800 dark:hover:text-zinc-300'
        ]"
      >
        <component :is="tab.icon" class="w-4 h-4 shrink-0" />
        <span>{{ tab.label }}</span>
      </button>
    </div>

    <!-- Основной контейнер содержимого -->
    <main class="flex-1 max-w-7xl w-full mx-auto p-6 space-y-6">
      
      <!-- 1. АНАЛИТИКА -->
      <section v-if="activeTab === 'analytics' && canManageCatalog" class="space-y-6">
        <!-- Панель выбора диапазона дат -->
        <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 bg-white dark:bg-zinc-900 p-4 rounded-2xl border border-zinc-200/80 dark:border-zinc-800/80 shadow-xs">
          <div>
            <h2 class="text-sm font-bold text-zinc-900 dark:text-zinc-100">Финансовые показатели и склад</h2>
            <p class="text-xs text-zinc-500 dark:text-zinc-400 mt-0.5">
              <template v-if="summary?.from && summary?.to">
                Данные с {{ new Date(summary.from).toLocaleDateString('ru-RU') }} по {{ new Date(summary.to).toLocaleDateString('ru-RU') }}
              </template>
              <template v-else>
                Сводный отчет эффективности
              </template>
            </p>
          </div>

          <div class="flex flex-wrap items-center gap-2">
            <div class="inline-flex p-1 bg-zinc-100 dark:bg-zinc-800/70 rounded-xl border border-zinc-200/60 dark:border-zinc-700/60">
              <button
                v-for="p in periods"
                :key="p.value"
                @click="selectPeriod(p.value)"
                :class="[
                  'px-3 py-1.5 rounded-lg text-xs font-semibold transition-all cursor-pointer',
                  selectedPeriod === p.value
                    ? 'bg-white dark:bg-zinc-900 text-zinc-900 dark:text-zinc-100 shadow-xs'
                    : 'text-zinc-500 hover:text-zinc-800 dark:hover:text-zinc-200'
                ]"
              >
                {{ p.label }}
              </button>
            </div>

            <!-- Ручной выбор дат -->
            <div v-if="selectedPeriod === 'custom'" class="flex items-center gap-1.5 bg-zinc-50 dark:bg-zinc-800/50 p-1 rounded-xl border border-zinc-200 dark:border-zinc-700">
              <input
                v-model="customDateFrom"
                type="date"
                class="px-2 py-1 text-xs rounded-lg border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-900 text-zinc-900 dark:text-zinc-100 focus:outline-none"
              />
              <span class="text-zinc-400 text-xs">—</span>
              <input
                v-model="customDateTo"
                type="date"
                class="px-2 py-1 text-xs rounded-lg border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-900 text-zinc-900 dark:text-zinc-100 focus:outline-none"
              />
              <button
                @click="loadAnalytics"
                class="px-2.5 py-1 text-xs font-bold text-white bg-emerald-600 hover:bg-emerald-700 rounded-lg transition cursor-pointer"
              >
                ОК
              </button>
            </div>
          </div>
        </div>

        <div v-if="loadingAnalytics" class="h-64 flex flex-col items-center justify-center text-zinc-400 gap-2">
          <div class="animate-spin w-6 h-6 border-2 border-emerald-500 border-t-transparent rounded-full"></div>
          <span class="text-xs font-medium">Расчёт финансовой сводки...</span>
        </div>

        <div v-else-if="summary" class="space-y-6">
          <!-- Складской капитал (Адаптирован под тему) -->
          <div class="p-6 rounded-3xl bg-linear-to-r from-emerald-50 via-white to-zinc-50 dark:from-emerald-950/40 dark:via-zinc-900 dark:to-zinc-900 border border-emerald-200/80 dark:border-emerald-500/20 shadow-xs">
            <div class="flex items-center justify-between mb-4">
              <div class="flex items-center gap-2">
                <div class="w-2 h-2 rounded-full bg-emerald-500 animate-pulse"></div>
                <span class="text-xs font-bold uppercase tracking-wider text-emerald-800 dark:text-emerald-400">
                  Оценка товарного остатка на складе
                </span>
              </div>
              <span class="text-[11px] text-zinc-500 dark:text-zinc-400">По всем активным партиям</span>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div class="p-4 rounded-2xl bg-white/80 dark:bg-zinc-800/40 border border-emerald-100 dark:border-zinc-700/60 shadow-2xs">
                <span class="text-xs font-medium text-zinc-500 dark:text-zinc-400 block mb-1">Себестоимость склада (Закупка)</span>
                <div class="text-2xl font-black text-zinc-900 dark:text-zinc-100">
                  {{ summary.currentStockPurchaseValue?.toLocaleString() || 0 }} ₽
                </div>
                <span class="text-[11px] text-zinc-400 mt-1 block">Замороженный капитал в цветах</span>
              </div>

              <div class="p-4 rounded-2xl bg-white/80 dark:bg-zinc-800/40 border border-emerald-100 dark:border-zinc-700/60 shadow-2xs">
                <span class="text-xs font-medium text-zinc-500 dark:text-zinc-400 block mb-1">Потенциальная выручка (Розница)</span>
                <div class="text-2xl font-black text-emerald-600 dark:text-emerald-400">
                  {{ summary.currentStockRetailValue?.toLocaleString() || 0 }} ₽
                </div>
                <span class="text-[11px] text-zinc-400 mt-1 block">При полной реализации</span>
              </div>

              <div class="p-4 rounded-2xl bg-emerald-600/5 dark:bg-emerald-500/10 border border-emerald-200 dark:border-emerald-500/30 shadow-2xs">
                <span class="text-xs font-semibold text-emerald-700 dark:text-emerald-400 block mb-1">Прогнозируемая прибыль склада</span>
                <div class="text-2xl font-black text-emerald-700 dark:text-emerald-300">
                  +{{ Math.max(0, (summary.currentStockRetailValue - summary.currentStockPurchaseValue)).toLocaleString() }} ₽
                </div>
                <span class="text-[11px] text-emerald-600/80 dark:text-emerald-400/80 mt-1 block font-medium">
                  Маржа склада: {{ summary.currentStockRetailValue > 0 ? Math.round(((summary.currentStockRetailValue - summary.currentStockPurchaseValue) / summary.currentStockRetailValue) * 100) : 0 }}%
                </span>
              </div>
            </div>
          </div>

          <!-- 5 Ключевых финансовых показателей -->
          <div>
            <h3 class="text-xs font-bold uppercase tracking-wider text-zinc-400 mb-3 px-1">
              Финансовые результаты за период
            </h3>

            <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-3.5">
              <!-- 1. Выручка -->
              <div class="p-4 rounded-2xl bg-white dark:bg-zinc-900 border border-zinc-200/80 dark:border-zinc-800/80 shadow-xs flex flex-col justify-between">
                <div>
                  <div class="flex items-center justify-between text-zinc-400 mb-1.5">
                    <span class="text-[11px] font-bold uppercase tracking-wider">Выручка</span>
                    <svg class="w-4 h-4 text-zinc-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                  </div>
                  <div class="text-xl font-black text-zinc-900 dark:text-zinc-50">
                    {{ summary.totalRevenue?.toLocaleString() }} ₽
                  </div>
                </div>
                <span class="text-[10px] text-zinc-400 mt-2 pt-2 border-t border-zinc-100 dark:border-zinc-800/60">
                  Принято по кассе
                </span>
              </div>

              <!-- 2. Себестоимость продаж -->
              <div class="p-4 rounded-2xl bg-white dark:bg-zinc-900 border border-zinc-200/80 dark:border-zinc-800/80 shadow-xs flex flex-col justify-between">
                <div>
                  <div class="flex items-center justify-between text-blue-500 mb-1.5">
                    <span class="text-[11px] font-bold uppercase tracking-wider">Себестоимость</span>
                    <svg class="w-4 h-4 text-blue-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                    </svg>
                  </div>
                  <div class="text-xl font-black text-blue-600 dark:text-blue-400">
                    {{ summary.costOfGoodsSold?.toLocaleString() }} ₽
                  </div>
                </div>
                <span class="text-[10px] text-zinc-400 mt-2 pt-2 border-t border-zinc-100 dark:border-zinc-800/60">
                  Закупка проданных цветов
                </span>
              </div>

              <!-- 3. Валовая прибыль -->
              <div class="p-4 rounded-2xl bg-white dark:bg-zinc-900 border border-zinc-200/80 dark:border-zinc-800/80 shadow-xs flex flex-col justify-between">
                <div>
                  <div class="flex items-center justify-between text-emerald-500 mb-1.5">
                    <span class="text-[11px] font-bold uppercase tracking-wider">Валовая прибыль</span>
                    <svg class="w-4 h-4 text-emerald-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
                    </svg>
                  </div>
                  <div class="text-xl font-black text-emerald-600 dark:text-emerald-400">
                    {{ summary.grossProfit?.toLocaleString() }} ₽
                  </div>
                </div>
                <div class="text-[10px] text-emerald-600 dark:text-emerald-400 font-bold mt-2 pt-2 border-t border-zinc-100 dark:border-zinc-800/60 flex justify-between">
                  <span>Маржинальность:</span>
                  <span>{{ summary.totalRevenue > 0 ? Math.round((summary.grossProfit / summary.totalRevenue) * 100) : 0 }}%</span>
                </div>
              </div>

              <!-- 4. ЧИСТАЯ ПРИБЫЛЬ -->
              <div class="p-4 rounded-2xl bg-emerald-500/10 dark:bg-emerald-500/15 border-2 border-emerald-500/40 shadow-xs flex flex-col justify-between">
                <div>
                  <div class="flex items-center justify-between text-emerald-700 dark:text-emerald-300 mb-1.5">
                    <span class="text-[11px] font-extrabold uppercase tracking-wider">Чистая прибыль</span>
                    <div class="w-2 h-2 rounded-full bg-emerald-500 animate-ping"></div>
                  </div>
                  <div class="text-2xl font-black text-emerald-700 dark:text-emerald-300">
                    {{ (summary.netProfit ?? (summary.grossProfit - summary.periodWriteOffPurchaseCost))?.toLocaleString() }} ₽
                  </div>
                </div>
                <span class="text-[10px] text-emerald-700/80 dark:text-emerald-300/80 font-medium mt-2 pt-2 border-t border-emerald-500/20">
                  Валовая прибыль − Списания
                </span>
              </div>

              <!-- 5. Денежный остаток в кассе -->
              <div class="p-4 rounded-2xl bg-white dark:bg-zinc-900 border border-zinc-200/80 dark:border-zinc-800/80 shadow-xs flex flex-col justify-between">
                <div>
                  <div class="flex items-center justify-between text-zinc-400 mb-1.5">
                    <span class="text-[11px] font-bold uppercase tracking-wider">Остаток в кассе</span>
                    <svg class="w-4 h-4 text-zinc-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z" />
                    </svg>
                  </div>
                  <div :class="['text-xl font-black', (summary.cashBalance ?? (summary.totalRevenue - summary.periodSuppliesPurchaseCost)) >= 0 ? 'text-zinc-900 dark:text-zinc-100' : 'text-rose-600']">
                    {{ (summary.cashBalance ?? (summary.totalRevenue - summary.periodSuppliesPurchaseCost)) >= 0 ? '+' : '' }}{{ (summary.cashBalance ?? (summary.totalRevenue - summary.periodSuppliesPurchaseCost))?.toLocaleString() }} ₽
                  </div>
                </div>
                <span class="text-[10px] text-zinc-400 mt-2 pt-2 border-t border-zinc-100 dark:border-zinc-800/60">
                  Выручка − Оплата поставок
                </span>
              </div>
            </div>
          </div>

          <!-- Движение товаров (Поставки vs Списания) -->
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-5">
            <!-- Поставки -->
            <div class="p-5 rounded-3xl bg-white dark:bg-zinc-900 border border-zinc-200/80 dark:border-zinc-800/80 shadow-xs space-y-4">
              <div class="flex items-center justify-between pb-3 border-b border-zinc-100 dark:border-zinc-800">
                <div class="flex items-center gap-2">
                  <div class="w-7 h-7 rounded-lg bg-blue-50 dark:bg-blue-950/60 text-blue-600 dark:text-blue-400 flex items-center justify-center">
                    <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                    </svg>
                  </div>
                  <h4 class="font-bold text-xs text-zinc-900 dark:text-zinc-100">Поставки за период</h4>
                </div>
                <button @click="switchTab('supplies-history')" class="text-[11px] font-semibold text-emerald-600 dark:text-emerald-400 hover:underline cursor-pointer">
                  В журнал &rarr;
                </button>
              </div>

              <div class="grid grid-cols-2 gap-3">
                <div class="p-3.5 rounded-xl bg-zinc-50 dark:bg-zinc-800/40 border border-zinc-100 dark:border-zinc-800">
                  <span class="text-[11px] text-zinc-500 dark:text-zinc-400 block mb-0.5">Потрачено на закупку</span>
                  <div class="text-base font-bold text-zinc-900 dark:text-zinc-100">
                    {{ summary.periodSuppliesPurchaseCost?.toLocaleString() }} ₽
                  </div>
                </div>
                <div class="p-3.5 rounded-xl bg-zinc-50 dark:bg-zinc-800/40 border border-zinc-100 dark:border-zinc-800">
                  <span class="text-[11px] text-zinc-500 dark:text-zinc-400 block mb-0.5">Потенциал розницы</span>
                  <div class="text-base font-bold text-emerald-600 dark:text-emerald-400">
                    {{ summary.periodSuppliesPotentialRevenue?.toLocaleString() }} ₽
                  </div>
                </div>
              </div>
            </div>

            <!-- Списания и скидки -->
            <div class="p-5 rounded-3xl bg-white dark:bg-zinc-900 border border-zinc-200/80 dark:border-zinc-800/80 shadow-xs space-y-4">
              <div class="flex items-center justify-between pb-3 border-b border-zinc-100 dark:border-zinc-800">
                <div class="flex items-center gap-2">
                  <div class="w-7 h-7 rounded-lg bg-rose-50 dark:bg-rose-950/60 text-rose-600 dark:text-rose-400 flex items-center justify-center">
                    <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                  </div>
                  <h4 class="font-bold text-xs text-zinc-900 dark:text-zinc-100">Потери, списания и уценки</h4>
                </div>
                <button @click="switchTab('writeoff')" class="text-[11px] font-semibold text-rose-500 hover:underline cursor-pointer">
                  В списания &rarr;
                </button>
              </div>

              <div class="grid grid-cols-2 gap-3">
                <div class="p-3.5 rounded-xl bg-rose-50/40 dark:bg-rose-950/20 border border-rose-100 dark:border-rose-900/40">
                  <span class="text-[11px] text-rose-700 dark:text-rose-400 block mb-0.5">Убыток (Закупка)</span>
                  <div class="text-base font-bold text-rose-600 dark:text-rose-400">
                    {{ summary.periodWriteOffPurchaseCost?.toLocaleString() }} ₽
                  </div>
                </div>
                <div class="p-3.5 rounded-xl bg-zinc-50 dark:bg-zinc-800/40 border border-zinc-100 dark:border-zinc-800">
                  <span class="text-[11px] text-zinc-500 dark:text-zinc-400 block mb-0.5">Упущенная розница</span>
                  <div class="text-base font-bold text-zinc-700 dark:text-zinc-300">
                    {{ summary.periodWriteOffRetailLoss?.toLocaleString() }} ₽
                  </div>
                </div>
              </div>

              <div class="p-3 rounded-xl bg-amber-50/50 dark:bg-amber-950/20 border border-amber-200/60 dark:border-amber-900/40 flex items-center justify-between text-xs">
                <span class="text-amber-800 dark:text-amber-300 font-medium">Скидки и уценки кассы:</span>
                <strong class="font-bold text-amber-700 dark:text-amber-400">−{{ summary.totalDiscountsGiven?.toLocaleString() }} ₽</strong>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- 2. КАТАЛОГ И ПАРТИИ -->
      <section v-if="activeTab === 'catalog'" class="p-6 rounded-3xl bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 shadow-xs space-y-4">
        <div class="flex flex-col sm:flex-row justify-between sm:items-center gap-3">
          <div>
            <h3 class="font-bold text-sm text-zinc-900 dark:text-zinc-100">Каталог цветов и активные партии</h3>
            <p class="text-xs text-zinc-500">
              {{ canManageCatalog ? 'Нажмите на розничную цену партии или название цветка для редактирования.' : 'Режим просмотра складских остатков.' }}
            </p>
          </div>

          <div class="flex items-center gap-2">
            <button
              @click="loadProducts"
              class="px-3 py-1.5 text-xs font-semibold rounded-xl border border-zinc-200 dark:border-zinc-700 hover:bg-zinc-50 dark:hover:bg-zinc-800 transition cursor-pointer flex items-center gap-1.5"
            >
              <svg class="w-3.5 h-3.5 text-zinc-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
              </svg>
              <span>Обновить</span>
            </button>
            <button
              v-if="canManageCatalog"
              @click="switchTab('product')"
              class="px-3.5 py-1.5 text-xs font-bold text-white bg-emerald-600 hover:bg-emerald-700 rounded-xl transition cursor-pointer shadow-xs flex items-center gap-1.5"
            >
              <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
              </svg>
              <span>Новый товар</span>
            </button>
          </div>
        </div>

        <!-- Поиск и фильтры -->
        <div class="flex flex-wrap items-center gap-3 pt-1">
          <div class="relative flex-1 min-w-[220px]">
            <span class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-zinc-400 text-xs">
              <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </span>
            <input
              v-model="catalogSearch"
              type="text"
              placeholder="Поиск по названию цветка..."
              class="w-full pl-8 pr-3 py-1.5 text-xs rounded-xl border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 text-zinc-900 dark:text-zinc-100 placeholder-zinc-400 focus:outline-none focus:ring-1 focus:ring-emerald-500"
            />
          </div>

          <div class="flex items-center gap-1.5 text-xs">
            <button
              v-for="f in [{ id: 'all', label: 'Все' }, { id: 'active', label: 'В продаже' }, { id: 'inStock', label: 'В наличии' }]"
              :key="f.id"
              @click="catalogFilter = f.id"
              :class="[
                'px-2.5 py-1 rounded-lg border transition cursor-pointer',
                catalogFilter === f.id
                  ? 'bg-zinc-800 text-white dark:bg-zinc-100 dark:text-zinc-900 border-transparent font-semibold'
                  : 'border-zinc-200 dark:border-zinc-800 text-zinc-500 hover:bg-zinc-50 dark:hover:bg-zinc-800'
              ]"
            >
              {{ f.label }}
            </button>
          </div>
        </div>

        <!-- Таблица каталога -->
        <div v-if="loadingProducts" class="py-12 text-center text-zinc-400 text-xs">
          Загрузка каталога товаров...
        </div>

        <div v-else-if="filteredCatalogProducts.length === 0" class="py-12 text-center text-zinc-400 text-xs">
          Товары не найдены.
        </div>

        <div v-else class="overflow-x-auto">
          <table class="w-full text-left text-xs">
            <thead>
              <tr class="border-b border-zinc-200 dark:border-zinc-800 text-zinc-400 uppercase tracking-wider font-semibold">
                <th class="py-3 px-2">Фото</th>
                <th class="py-3 px-2">Название (Клик для смены)</th>
                <th class="py-3 px-2">Остаток</th>
                <th class="py-3 px-2">Партии на складе (Закупка → Розница)</th>
                <th class="py-3 px-2">Статус</th>
                <th v-if="canManageCatalog" class="py-3 px-2 text-right">Действия</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-zinc-100 dark:divide-zinc-800/50">
              <tr v-for="product in filteredCatalogProducts" :key="product.id" class="hover:bg-zinc-50/50 dark:hover:bg-zinc-800/30">
                <td class="py-2.5 px-2 w-12">
                  <img
                    v-if="product.imageUrl"
                    :src="apiBaseUrl + product.imageUrl"
                    class="w-9 h-9 object-cover rounded-lg shadow-2xs"
                  />
                  <div v-else class="w-9 h-9 rounded-lg bg-zinc-100 dark:bg-zinc-800 flex items-center justify-center text-zinc-400">
                    <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                    </svg>
                  </div>
                </td>

                <!-- Инлайн-редактирование названия товара -->
                <td class="py-2.5 px-2 font-medium text-zinc-900 dark:text-zinc-100">
                  <div v-if="editingProductNameId === product.id" class="flex items-center gap-1">
                    <input
                      v-model="tempProductName"
                      @keydown.enter="saveProductName(product.id)"
                      @keydown.esc="cancelEditProductName"
                      type="text"
                      class="px-2 py-1 text-xs rounded-lg border border-emerald-500 bg-white dark:bg-zinc-900 text-zinc-900 dark:text-zinc-100 focus:outline-none"
                      autofocus
                    />
                    <button @click="saveProductName(product.id)" class="text-emerald-500 font-bold hover:text-emerald-600 p-1 cursor-pointer" title="Сохранить">✓</button>
                    <button @click="cancelEditProductName" class="text-zinc-400 hover:text-zinc-600 p-1 cursor-pointer" title="Отмена">✕</button>
                  </div>
                  <div v-else class="flex items-center gap-1.5 group">
                    <span>{{ product.name }}</span>
                    <button
                      v-if="canManageCatalog"
                      @click="startEditProductName(product)"
                      class="opacity-0 group-hover:opacity-100 text-zinc-400 hover:text-zinc-600 dark:hover:text-zinc-200 transition cursor-pointer"
                      title="Изменить название цветка"
                    >
                      <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
                      </svg>
                    </button>
                  </div>
                </td>

                <td class="py-2.5 px-2 font-bold">
                  <span :class="product.stock <= 0 ? 'text-rose-500' : product.stock <= 5 ? 'text-amber-500' : 'text-zinc-700 dark:text-zinc-300'">
                    {{ product.stock }} шт
                  </span>
                </td>

                <!-- Партии со сменой цен -->
                <td class="py-2.5 px-2">
                  <div v-if="product.activeBatches && product.activeBatches.length > 0" class="flex flex-wrap gap-1.5 items-center">
                    <div
                      v-for="b in product.activeBatches"
                      :key="b.batchId"
                      class="inline-flex items-center gap-1.5 px-2 py-1 rounded-lg bg-zinc-100 dark:bg-zinc-800 border border-zinc-200 dark:border-zinc-700 text-[11px]"
                    >
                      <span class="font-bold">{{ b.remainingQuantity }} шт</span>
                      <span v-if="canManageCatalog" class="text-zinc-400">(зак. {{ b.purchasePrice }} ₽)</span>
                      <span>→</span>
                      
                      <!-- Редактирование цены партии -->
                      <div v-if="canManageCatalog && editingBatchId === b.batchId" class="flex items-center gap-1">
                        <input
                          v-model.number="tempBatchPrice"
                          type="number"
                          step="1"
                          min="0"
                          class="w-16 px-1.5 py-0.5 rounded border border-emerald-500 bg-white dark:bg-zinc-900 text-xs font-bold focus:outline-none"
                        />
                        <button @click="saveBatchPrice(b.batchId)" class="text-emerald-500 font-bold hover:text-emerald-600 cursor-pointer" title="Сохранить">✓</button>
                        <button @click="cancelEditBatchPrice" class="text-zinc-400 hover:text-zinc-600 cursor-pointer" title="Отмена">✕</button>
                      </div>

                      <div v-else class="flex items-center gap-1">
                        <strong class="text-emerald-600 dark:text-emerald-400">{{ b.sellingPrice }} ₽</strong>
                        <button
                          v-if="canManageCatalog"
                          @click="startEditBatchPrice(b)"
                          class="text-zinc-400 hover:text-zinc-600 dark:hover:text-zinc-200 cursor-pointer text-[10px]"
                          title="Изменить розничную цену партии"
                        >
                          <svg class="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
                          </svg>
                        </button>
                      </div>
                    </div>
                  </div>
                  <span v-else class="text-[10px] text-zinc-400 italic">Активных партий нет</span>
                </td>

                <td class="py-2.5 px-2">
                  <span
                    :class="[
                      'px-2 py-0.5 rounded-full text-[10px] font-bold',
                      product.isActive 
                        ? 'bg-emerald-50 text-emerald-600 dark:bg-emerald-950 dark:text-emerald-400' 
                        : 'bg-zinc-100 text-zinc-500 dark:bg-zinc-800 dark:text-zinc-400'
                    ]"
                  >
                    {{ product.isActive ? 'В продаже' : 'Снят с продажи' }}
                  </span>
                </td>

                <!-- Действия: снятие только при stock == 0, или возврат на витрину -->
                <td v-if="canManageCatalog" class="py-2.5 px-2 text-right">
                  <button
                    v-if="product.isActive"
                    @click="toggleProductStatus(product)"
                    :disabled="product.stock > 0"
                    :title="product.stock > 0 ? `Нельзя снять товар с продажи, пока на складе есть остаток (${product.stock} шт.)` : 'Снять товар с продажи'"
                    class="text-xs font-semibold text-rose-500 hover:text-rose-700 disabled:opacity-30 disabled:cursor-not-allowed cursor-pointer"
                  >
                    Снять
                  </button>
                  <button
                    v-else
                    @click="toggleProductStatus(product)"
                    class="text-xs font-semibold text-emerald-600 hover:text-emerald-700 cursor-pointer"
                  >
                    Вернуть в продажу
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <!-- 3. НОВЫЙ ТОВАР -->
      <section v-if="activeTab === 'product' && canManageCatalog" class="max-w-xl mx-auto p-6 rounded-3xl bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 shadow-xs space-y-4">
        <div>
          <h3 class="font-bold text-sm text-zinc-900 dark:text-zinc-100">Создание нового товара / позиции</h3>
          <p class="text-xs text-zinc-500">Товар сразу публикуется на кассе с созданной начальной партией.</p>
        </div>

        <form @submit.prevent="submitProduct" class="space-y-4">
          <div>
            <label class="block text-xs font-semibold text-zinc-500 mb-1">Название цветка / упаковки</label>
            <input 
              v-model="productForm.name" 
              type="text" 
              required 
              placeholder="Роза Ред Наоми 60см" 
              class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800 text-zinc-900 dark:text-zinc-100 focus:outline-none focus:ring-1 focus:ring-emerald-500" 
            />
          </div>

          <div class="grid grid-cols-3 gap-3">
            <div>
              <label class="block text-xs font-semibold text-zinc-500 mb-1">Закупка 1 шт (₽)</label>
              <input 
                v-model.number="productForm.purchasePrice" 
                type="number" 
                min="0" 
                step="0.5" 
                required 
                class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800 text-zinc-900 dark:text-zinc-100 focus:outline-none focus:ring-1 focus:ring-emerald-500" 
              />
            </div>
            <div>
              <label class="block text-xs font-semibold text-zinc-500 mb-1">Розница 1 шт (₽)</label>
              <input 
                v-model.number="productForm.price" 
                type="number" 
                min="0" 
                step="0.5" 
                required 
                class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800 text-zinc-900 dark:text-zinc-100 focus:outline-none focus:ring-1 focus:ring-emerald-500" 
              />
            </div>
            <div>
              <label class="block text-xs font-semibold text-zinc-500 mb-1">Начальный остаток (шт)</label>
              <input 
                v-model.number="productForm.currentQuantity" 
                type="number" 
                min="0" 
                required 
                class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800 text-zinc-900 dark:text-zinc-100 focus:outline-none focus:ring-1 focus:ring-emerald-500" 
              />
            </div>
          </div>

          <div>
            <label class="block text-xs font-semibold text-zinc-500 mb-1">Фотография товара</label>
            <div class="border-2 border-dashed border-zinc-200 dark:border-zinc-800 rounded-2xl p-4 text-center bg-zinc-50/50 dark:bg-zinc-800/30">
              <input type="file" accept="image/*" @change="handleImageSelection" ref="fileInput" class="hidden" />
              <div v-if="imagePreview" class="flex flex-col items-center gap-2">
                <img :src="imagePreview" class="w-24 h-24 object-cover rounded-xl shadow-xs" />
                <button type="button" @click="resetImage" class="text-xs text-rose-500 font-semibold hover:underline cursor-pointer">✕ Удалить фото</button>
              </div>
              <div v-else @click="$refs.fileInput.click()" class="cursor-pointer py-4 text-xs text-zinc-400 hover:text-zinc-600 dark:hover:text-zinc-300 flex flex-col items-center gap-1.5">
                <svg class="w-6 h-6 text-zinc-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M3 9a2 2 0 012-2h.93a2 2 0 001.664-.89l.812-1.22A2 2 0 0110.07 4h3.86a2 2 0 011.664.89l.812 1.22A2 2 0 0018.07 7H19a2 2 0 012 2v9a2 2 0 01-2 2H5a2 2 0 01-2-2V9z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15 13a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
                <span>Нажмите для выбора фотографии (до 5 МБ)</span>
              </div>
            </div>
          </div>

          <button 
            type="submit" 
            :disabled="isSubmitting" 
            class="w-full py-3 rounded-xl bg-emerald-600 hover:bg-emerald-700 text-white font-bold text-xs shadow-md transition cursor-pointer disabled:opacity-50"
          >
            {{ isSubmitting ? 'Сохранение...' : 'Создать товар с первой партией' }}
          </button>
        </form>
      </section>

      <!-- 4. ПРОДАЖИ -->
      <section v-if="activeTab === 'orders'" class="p-6 rounded-3xl bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 shadow-xs space-y-4">
        <h3 class="font-bold text-sm text-zinc-900 dark:text-zinc-100">Журнал продаж</h3>
        <div v-if="loadingOrders" class="py-8 text-center text-zinc-400 text-xs">Загрузка чеков...</div>
        <div v-else-if="ordersList.length === 0" class="py-8 text-center text-zinc-400 text-xs">Чеков пока нет.</div>
        <div v-else class="overflow-x-auto">
          <table class="w-full text-left text-xs">
            <thead>
              <tr class="border-b border-zinc-200 dark:border-zinc-800 text-zinc-400 uppercase tracking-wider font-semibold">
                <th class="py-3 px-2">Дата</th>
                <th class="py-3 px-2">Номер</th>
                <th class="py-3 px-2">Сумма</th>
                <th class="py-3 px-2 text-right">Детали</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-zinc-100 dark:divide-zinc-800/50">
              <tr v-for="order in ordersList" :key="order.orderId" class="hover:bg-zinc-50/50 dark:hover:bg-zinc-800/30">
                <td class="py-3 px-2">{{ new Date(order.createdAt).toLocaleString('ru-RU') }}</td>
                <td class="py-3 px-2 font-mono text-[11px]">#{{ order.orderId?.substring(0, 8) }}</td>
                <td class="py-3 px-2 font-bold text-emerald-600">{{ order.totalAmount?.toLocaleString() }} ₽</td>
                <td class="py-3 px-2 text-right">
                  <button @click="selectedOrder = order" class="px-2.5 py-1 text-xs font-semibold rounded-lg bg-zinc-100 dark:bg-zinc-800 hover:bg-zinc-200 cursor-pointer">
                    Просмотр
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <!-- 5. ЖУРНАЛ ПОСТАВОК -->
      <section v-if="activeTab === 'supplies-history'" class="p-6 rounded-3xl bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 shadow-xs space-y-4">
        <div class="flex justify-between items-center">
          <h3 class="font-bold text-sm text-zinc-900 dark:text-zinc-100">Журнал поступления партий</h3>
          <button @click="showSupplyModal = true" class="px-3.5 py-1.5 text-xs font-bold text-white bg-emerald-600 hover:bg-emerald-700 rounded-xl cursor-pointer shadow-xs flex items-center gap-1.5">
            <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" /></svg>
            <span>Принять поставку</span>
          </button>
        </div>

        <div v-if="loadingSupplies" class="py-8 text-center text-zinc-400 text-xs">Загрузка журнала поставок...</div>
        <div v-else-if="suppliesList.length === 0" class="py-8 text-center text-zinc-400 text-xs">Поставок пока не было.</div>
        <div v-else class="overflow-x-auto">
          <table class="w-full text-left text-xs">
            <thead>
              <tr class="border-b border-zinc-200 dark:border-zinc-800 text-zinc-400 uppercase tracking-wider font-semibold">
                <th class="py-3 px-2">Дата</th>
                <th class="py-3 px-2">Товар</th>
                <th class="py-3 px-2">Количество</th>
                <th class="py-3 px-2">Закупка 1 шт</th>
                <th class="py-3 px-2">Розница 1 шт</th>
                <th class="py-3 px-2">Сумма закупки</th>
                <th class="py-3 px-2">Потенциал розницы</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-zinc-100 dark:divide-zinc-800/50">
              <tr v-for="s in suppliesList" :key="s.id" class="hover:bg-zinc-50/50 dark:hover:bg-zinc-800/30">
                <td class="py-3 px-2">{{ new Date(s.createdAt).toLocaleString('ru-RU') }}</td>
                <td class="py-3 px-2 font-bold">{{ s.productName }}</td>
                <td class="py-3 px-2">{{ s.quantity }} шт</td>
                <td class="py-3 px-2 text-zinc-500">{{ s.purchasePrice }} ₽</td>
                <td class="py-3 px-2 font-semibold text-emerald-500">{{ s.sellingPrice }} ₽</td>
                <td class="py-3 px-2 font-bold">{{ (s.quantity * s.purchasePrice).toLocaleString() }} ₽</td>
                <td class="py-3 px-2 font-bold text-emerald-500">{{ (s.quantity * s.sellingPrice).toLocaleString() }} ₽</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <!-- 6. ЖУРНАЛ СПИСАНИЙ (Суммы закупки и розницы) -->
      <section v-if="activeTab === 'writeoff'" class="p-6 rounded-3xl bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 shadow-xs space-y-4">
        <h3 class="font-bold text-sm text-zinc-900 dark:text-zinc-100">Журнал списаний цветов</h3>
        <div v-if="loadingWriteOffs" class="py-8 text-center text-zinc-400 text-xs">Загрузка списаний...</div>
        <div v-else-if="writeOffsHistory.length === 0" class="py-8 text-center text-zinc-400 text-xs">Списаний не зафиксировано.</div>
        <div v-else class="overflow-x-auto">
          <table class="w-full text-left text-xs">
            <thead>
              <tr class="border-b border-zinc-200 dark:border-zinc-800 text-zinc-400 uppercase tracking-wider font-semibold">
                <th class="py-3 px-2">Дата</th>
                <th class="py-3 px-2">Товар</th>
                <th class="py-3 px-2">Количество</th>
                <th class="py-3 px-2">Закупка (1 шт → Сумма)</th>
                <th class="py-3 px-2">Розница (1 шт → Сумма)</th>
                <th class="py-3 px-2">Причина</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-zinc-100 dark:divide-zinc-800/50">
              <tr v-for="item in writeOffsHistory" :key="item.id" class="hover:bg-zinc-50/50 dark:hover:bg-zinc-800/30">
                <td class="py-2.5 px-2 text-zinc-400 font-mono text-[11px]">{{ new Date(item.createdAt).toLocaleString('ru-RU') }}</td>
                <td class="py-2.5 px-2 font-medium">{{ item.productName }}</td>
                <td class="py-2.5 px-2 font-bold">{{ item.quantity }} шт</td>
                <td class="py-2.5 px-2">
                  <span class="text-zinc-500">{{ item.purchasePriceAtWriteOff }} ₽</span>
                  <span class="text-zinc-400 mx-1">→</span>
                  <strong class="text-rose-500 font-bold">
                    {{ (item.totalPurchaseCost ?? (item.quantity * item.purchasePriceAtWriteOff)).toLocaleString() }} ₽
                  </strong>
                </td>
                <td class="py-2.5 px-2">
                  <span class="text-zinc-500">{{ item.retailPriceAtWriteOff ?? item.purchasePriceAtWriteOff }} ₽</span>
                  <span class="text-zinc-400 mx-1">→</span>
                  <strong class="text-zinc-700 dark:text-zinc-300 font-bold">
                    {{ (item.totalRetailLoss ?? (item.quantity * (item.retailPriceAtWriteOff ?? item.purchasePriceAtWriteOff))).toLocaleString() }} ₽
                  </strong>
                </td>
                <td class="py-2.5 px-2">
                  <span class="px-2 py-0.5 rounded-full bg-rose-50 text-rose-600 dark:bg-rose-950 dark:text-rose-400 font-medium text-[11px]">
                    {{ item.reason }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <!-- 7. СОТРУДНИКИ (Только Admin) -->
      <section v-if="activeTab === 'users' && authStore.isAdmin" class="p-6 rounded-3xl bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 shadow-xs space-y-4">
        <div class="flex justify-between items-center">
          <div>
            <h3 class="font-bold text-sm text-zinc-900 dark:text-zinc-100">Управление сотрудниками</h3>
            <p class="text-xs text-zinc-500">Доступно только системному администратору.</p>
          </div>
          <button @click="showNewUserModal = true" class="px-3.5 py-1.5 text-xs font-bold text-white bg-emerald-600 hover:bg-emerald-700 rounded-xl cursor-pointer shadow-xs flex items-center gap-1.5">
            <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" /></svg>
            <span>Новый сотрудник</span>
          </button>
        </div>

        <div v-if="loadingUsers" class="py-8 text-center text-zinc-400 text-xs">Загрузка списка пользователей...</div>
        <div v-else class="overflow-x-auto">
          <table class="w-full text-left text-xs">
            <thead>
              <tr class="border-b border-zinc-200 dark:border-zinc-800 text-zinc-400 uppercase tracking-wider font-semibold">
                <th class="py-3 px-2">Логин</th>
                <th class="py-3 px-2">Роль</th>
                <th class="py-3 px-2">Статус</th>
                <th class="py-3 px-2 text-right">Действия</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-zinc-100 dark:divide-zinc-800/50">
              <tr v-for="u in usersList" :key="u.id" class="hover:bg-zinc-50/50 dark:hover:bg-zinc-800/30">
                <td class="py-3 px-2 font-bold">{{ u.username }}</td>
                <td class="py-3 px-2">
                  <span
                    :class="[
                      'px-2 py-0.5 rounded-full text-[10px] font-bold',
                      u.roleName === 'Admin' ? 'bg-rose-100 text-rose-700 dark:bg-rose-950 dark:text-rose-300' :
                      u.roleName === 'Owner' ? 'bg-purple-100 text-purple-700 dark:bg-purple-950 dark:text-purple-300' :
                      'bg-blue-100 text-blue-700 dark:bg-blue-950 dark:text-blue-300'
                    ]"
                  >
                    {{ u.roleName === 'Admin' ? 'Администратор' : u.roleName === 'Owner' ? 'Владелец' : 'Кассир' }}
                  </span>
                </td>
                <td class="py-3 px-2">
                  <span :class="['px-2 py-0.5 rounded-full text-[10px] font-bold', u.isActive ? 'bg-emerald-50 text-emerald-600 dark:bg-emerald-950 dark:text-emerald-400' : 'bg-rose-50 text-rose-600 dark:bg-rose-950 dark:text-rose-400']">
                    {{ u.isActive ? 'Активен' : 'Заблокирован' }}
                  </span>
                </td>
                <td class="py-3 px-2 text-right space-x-2">
                  <button @click="openResetPasswordModal(u)" class="px-2.5 py-1 text-xs font-semibold rounded-lg bg-zinc-100 dark:bg-zinc-800 hover:bg-zinc-200 cursor-pointer">
                    Сброс пароля
                  </button>
                  <button
                    v-if="u.id !== authStore.user?.id"
                    @click="toggleUserBlock(u)"
                    :class="['px-2.5 py-1 text-xs font-semibold rounded-lg cursor-pointer transition', u.isActive ? 'text-rose-500 hover:bg-rose-50 dark:hover:bg-rose-950/40' : 'text-emerald-500 hover:bg-emerald-50 dark:hover:bg-emerald-950/40']"
                  >
                    {{ u.isActive ? 'Заблокировать' : 'Разблокировать' }}
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <!-- 8. АУДИТ И ЛОГИ (Только для системного администратора) -->
      <section v-if="activeTab === 'audit' && authStore.isAdmin" class="space-y-6">

        <!-- Скрытый блок системного обслуживания хранилища -->
        <div class="p-5 rounded-3xl bg-zinc-900 text-zinc-100 border border-zinc-800 shadow-md flex flex-col sm:flex-row sm:items-center justify-between gap-4">
          <div class="space-y-1">
            <div class="flex items-center gap-2">
              <span class="w-2 h-2 rounded-full bg-amber-400"></span>
              <h4 class="font-bold text-xs uppercase tracking-wider text-zinc-200">
                Инструменты обслуживания сервера
              </h4>
            </div>
            <p class="text-xs text-zinc-400">
              Пакетная конвертация всех загруженных изображений каталога в WebP (600px, 75% quality). Снижает объем диска и устраняет лаги в кассе.
            </p>
          </div>

          <button
            @click="triggerBulkImageOptimization"
            :disabled="isOptimizingImages"
            class="px-4 py-2.5 rounded-xl bg-amber-500/10 hover:bg-amber-500/20 text-amber-300 border border-amber-500/30 text-xs font-bold transition flex items-center justify-center gap-2 cursor-pointer shrink-0 disabled:opacity-40"
          >
            <span v-if="isOptimizingImages" class="animate-spin w-3.5 h-3.5 border-2 border-amber-400 border-t-transparent rounded-full"></span>
            <svg v-else class="w-4 h-4 text-amber-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19.428 15.428a2 2 0 00-1.022-.547l-2.387-.477a6 6 0 00-3.86.517l-.318.158a6 6 0 01-3.86.517L6.05 15.21a2 2 0 00-1.806.547M8 4h8l-1 1v5.172a2 2 0 00.586 1.414l5 5c1.26 1.26.367 3.414-1.415 3.414H4.828c-1.782 0-2.674-2.154-1.414-3.414l5-5A2 2 0 009 10.172V5L8 4z" />
            </svg>
            <span>{{ isOptimizingImages ? 'Оптимизация файлов...' : 'Сжать все фото в базе' }}</span>
          </button>
        </div>

        <!-- Таблица журнала аудита -->
        <div class="p-6 rounded-3xl bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 shadow-xs space-y-4">
          <div class="flex items-center justify-between">
            <h3 class="font-bold text-sm text-zinc-900 dark:text-zinc-100">Журнал безопасности и аудита действий</h3>
            <button
              @click="loadAuditLogs"
              class="text-xs text-zinc-400 hover:text-zinc-600 dark:hover:text-zinc-200 cursor-pointer"
            >
              Обновить логи
            </button>
          </div>

          <div v-if="loadingAudit" class="py-8 text-center text-zinc-400 text-xs">Загрузка журнала безопасности...</div>
          <div v-else-if="auditLogs.length === 0" class="py-8 text-center text-zinc-400 text-xs">Логи отсутствуют.</div>
          <div v-else class="overflow-x-auto">
            <table class="w-full text-left text-xs">
              <thead>
                <tr class="border-b border-zinc-200 dark:border-zinc-800 text-zinc-400 uppercase tracking-wider font-semibold">
                  <th class="py-3 px-2">Время</th>
                  <th class="py-3 px-2">Пользователь</th>
                  <th class="py-3 px-2">Действие</th>
                  <th class="py-3 px-2">Объект</th>
                  <th class="py-3 px-2">Подробности</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-zinc-100 dark:divide-zinc-800/50">
                <tr v-for="log in auditLogs" :key="log.id" class="hover:bg-zinc-50/50 dark:hover:bg-zinc-800/30">
                  <td class="py-2.5 px-2 text-zinc-400 font-mono text-[11px]">{{ new Date(log.createdAt).toLocaleString('ru-RU') }}</td>
                  <td class="py-2.5 px-2 font-bold">{{ log.username }}</td>
                  <td class="py-2.5 px-2 font-mono text-[11px] text-blue-500 font-semibold">{{ log.action }}</td>
                  <td class="py-2.5 px-2 text-zinc-500">{{ log.entityName }}</td>
                  <td class="py-2.5 px-2 text-zinc-700 dark:text-zinc-300">{{ log.details }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </section>
    </main>

    <!-- Модалка приёма партии -->
    <div v-if="showSupplyModal" class="fixed inset-0 z-50 bg-black/60 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 rounded-3xl w-full max-w-md p-6 shadow-2xl space-y-4">
        <h3 class="font-bold text-sm text-zinc-900 dark:text-zinc-100">Приёмка новой партии товара</h3>
        <form @submit.prevent="submitSupply" class="space-y-3">
          <div>
            <label class="block text-xs font-semibold text-zinc-500 mb-1">Товар</label>
            <select v-model="supplyForm.productId" required class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800">
              <option value="" disabled>-- Выберите позицию --</option>
              <option v-for="p in products" :key="p.id" :value="p.id">{{ p.name }} (На складе: {{ p.stock }} шт)</option>
            </select>
          </div>
          <div>
            <label class="block text-xs font-semibold text-zinc-500 mb-1">Количество поступивших (шт)</label>
            <input v-model.number="supplyForm.quantity" type="number" min="1" required class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800" />
          </div>
          <div class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-xs font-semibold text-zinc-500 mb-1">Закупка 1 шт (₽)</label>
              <input v-model.number="supplyForm.purchasePrice" type="number" step="0.5" min="0" required class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800" />
            </div>
            <div>
              <label class="block text-xs font-semibold text-zinc-500 mb-1">Розница 1 шт (₽)</label>
              <input v-model.number="supplyForm.sellingPrice" type="number" step="0.5" min="0" required class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800" />
            </div>
          </div>
          <div class="p-3 bg-zinc-100 dark:bg-zinc-800 rounded-xl text-xs flex justify-between font-medium">
            <span>Себестоимость партии:</span>
            <strong>{{ (supplyForm.quantity * supplyForm.purchasePrice).toLocaleString() }} ₽</strong>
          </div>
          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showSupplyModal = false" class="px-4 py-2 text-xs font-semibold text-zinc-600 rounded-xl cursor-pointer">Отмена</button>
            <button type="submit" :disabled="isSubmitting" class="px-5 py-2 text-xs font-bold text-white bg-emerald-600 hover:bg-emerald-700 rounded-xl cursor-pointer">Оприходовать</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Модалка просмотра чека -->
    <div v-if="selectedOrder" class="fixed inset-0 z-50 bg-black/60 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 rounded-3xl w-full max-w-md p-6 shadow-2xl space-y-4">
        <div class="flex justify-between items-center border-b border-zinc-200 dark:border-zinc-800 pb-3">
          <div>
            <h3 class="font-bold text-sm">Чек #{{ (selectedOrder.orderId || selectedOrder.id)?.substring(0, 8) }}</h3>
            <span class="text-[11px] text-zinc-400">{{ new Date(selectedOrder.createdAt).toLocaleString('ru-RU') }}</span>
          </div>
          <button @click="selectedOrder = null" class="text-zinc-400 hover:text-zinc-600 cursor-pointer">✕</button>
        </div>

        <div class="space-y-2 text-xs max-h-72 overflow-y-auto pr-1">
          <div v-for="comp in selectedOrder.compositions" :key="comp.name" class="p-2.5 bg-pink-50/50 dark:bg-pink-950/20 rounded-xl space-y-1">
            <span class="font-bold text-pink-700 dark:text-pink-300">Букет: {{ comp.name }} (Сборка: {{ comp.assemblyPrice }} ₽)</span>
            <div v-for="cItem in comp.items" :key="cItem.productName" class="flex justify-between text-zinc-500">
              <span>{{ cItem.productName }} × {{ cItem.quantity }}</span>
              <span>{{ cItem.totalPrice }} ₽</span>
            </div>
          </div>

          <div v-for="item in selectedOrder.singleItems" :key="item.productName" class="flex justify-between p-2 rounded-lg bg-zinc-50 dark:bg-zinc-800/40">
            <div>
              <span>{{ item.productName }} × {{ item.quantity }}</span>
              <span v-if="item.discountInfo && item.discountInfo !== 'Без скидки'" class="ml-1 text-amber-500 font-semibold">({{ item.discountInfo }})</span>
            </div>
            <span class="font-semibold">{{ item.totalPrice }} ₽</span>
          </div>
        </div>

        <div class="pt-3 border-t border-zinc-200 dark:border-zinc-800 flex justify-between items-baseline font-bold">
          <span class="text-xs uppercase text-zinc-500">Итого</span>
          <span class="text-xl text-emerald-600">{{ selectedOrder.totalAmount?.toLocaleString() }} ₽</span>
        </div>
      </div>
    </div>

    <!-- Модалка создания сотрудника -->
    <div v-if="showNewUserModal" class="fixed inset-0 z-50 bg-black/60 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 rounded-3xl w-full max-w-sm p-6 shadow-2xl space-y-4">
        <h3 class="font-bold text-sm">Добавить нового сотрудника</h3>
        <form @submit.prevent="submitCreateUser" class="space-y-3">
          <div>
            <label class="block text-xs font-semibold text-zinc-500 mb-1">Логин</label>
            <input v-model="newUserForm.username" type="text" required class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800" />
          </div>
          <div>
            <label class="block text-xs font-semibold text-zinc-500 mb-1">Пароль</label>
            <input v-model="newUserForm.password" type="password" required minlength="6" class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800" />
          </div>
          <div>
            <label class="block text-xs font-semibold text-zinc-500 mb-1">Роль в системе</label>
            <select v-model="newUserForm.roleName" class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800">
              <option value="Cashier">Кассир (Касса, поставки, списания)</option>
              <option value="Owner">Владелец (Аналитика, склад, цены)</option>
              <option value="Admin">Администратор (Полный доступ и управление)</option>
            </select>
          </div>
          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showNewUserModal = false" class="px-4 py-2 text-xs font-semibold text-zinc-600 rounded-xl cursor-pointer">Отмена</button>
            <button type="submit" class="px-5 py-2 text-xs font-bold text-white bg-emerald-600 hover:bg-emerald-700 rounded-xl cursor-pointer">Создать</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Модалка сброса пароля -->
    <div v-if="resetPasswordUser" class="fixed inset-0 z-50 bg-black/60 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 rounded-3xl w-full max-w-sm p-6 shadow-2xl space-y-4">
        <h3 class="font-bold text-sm">Сброс пароля: {{ resetPasswordUser.username }}</h3>
        <form @submit.prevent="submitResetPassword" class="space-y-3">
          <div>
            <label class="block text-xs font-semibold text-zinc-500 mb-1">Новый пароль</label>
            <input v-model="newPasswordValue" type="password" required minlength="6" class="w-full px-3 py-2 text-xs rounded-xl border border-zinc-300 dark:border-zinc-700 bg-white dark:bg-zinc-800" />
          </div>
          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="resetPasswordUser = null" class="px-4 py-2 text-xs font-semibold text-zinc-600 rounded-xl cursor-pointer">Отмена</button>
            <button type="submit" class="px-5 py-2 text-xs font-bold text-white bg-emerald-600 hover:bg-emerald-700 rounded-xl cursor-pointer">Сменить</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch, h } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useThemeStore } from '../stores/theme'
import apiClient from '../api/axios'

const router = useRouter()
const authStore = useAuthStore()
const themeStore = useThemeStore()

const apiBaseUrl = import.meta.env.VITE_UPLOADS_URL || ''

// Стартовая вкладка
const activeTab = ref(authStore.isCashier ? 'orders' : 'analytics')

// Флаги загрузки
const loadingAnalytics = ref(false)
const loadingProducts = ref(false)
const loadingOrders = ref(false)
const loadingSupplies = ref(false)
const loadingWriteOffs = ref(false)
const loadingAudit = ref(false)
const loadingUsers = ref(false)
const isSubmitting = ref(false)

// Списки данных
const summary = ref(null)
const products = ref([])
const ordersList = ref([])
const suppliesList = ref([])
const writeOffsHistory = ref([])
const auditLogs = ref([])
const usersList = ref([])
const selectedOrder = ref(null)

// Фильтры каталога
const catalogSearch = ref('')
const catalogFilter = ref('all') // 'all' | 'active' | 'inStock'

// Настройка периодов аналитики
const selectedPeriod = ref('month')
const periods = [
  { label: 'Сегодня', value: 'today' },
  { label: '7 дней', value: 'week' },
  { label: '30 дней', value: 'month' },
  { label: 'Все время', value: 'all' },
  { label: 'Свой период', value: 'custom' }
]
const customDateFrom = ref('')
const customDateTo = ref('')

const canManageCatalog = computed(() => authStore.isOwner || authStore.isAdmin)

// Иконки для вкладок
const ChartIcon = () => h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
  h('path', { strokeLinecap: 'round', strokeLinejoin: 'round', strokeWidth: '2', d: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z' })
])
const OrdersIcon = () => h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
  h('path', { strokeLinecap: 'round', strokeLinejoin: 'round', strokeWidth: '2', d: 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z' })
])
const SupplyIcon = () => h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
  h('path', { strokeLinecap: 'round', strokeLinejoin: 'round', strokeWidth: '2', d: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4' })
])
const CatalogIcon = () => h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
  h('path', { strokeLinecap: 'round', strokeLinejoin: 'round', strokeWidth: '2', d: 'M4 6h16M4 10h16M4 14h16M4 18h16' })
])
const ProductAddIcon = () => h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
  h('path', { strokeLinecap: 'round', strokeLinejoin: 'round', strokeWidth: '2', d: 'M12 4v16m8-8H4' })
])
const WriteoffIcon = () => h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
  h('path', { strokeLinecap: 'round', strokeLinejoin: 'round', strokeWidth: '2', d: 'M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16' })
])
const UsersIcon = () => h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
  h('path', { strokeLinecap: 'round', strokeLinejoin: 'round', strokeWidth: '2', d: 'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z' })
])
const AuditIcon = () => h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
  h('path', { strokeLinecap: 'round', strokeLinejoin: 'round', strokeWidth: '2', d: 'M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z' })
])

// Динамические вкладки в зависимости от роли
const visibleTabs = computed(() => {
  if (authStore.isCashier) {
    return [
      { id: 'orders', label: 'Продажи', icon: OrdersIcon },
      { id: 'supplies-history', label: 'Журнал поставок', icon: SupplyIcon },
      { id: 'writeoff', label: 'Журнал списаний', icon: WriteoffIcon },
      { id: 'catalog', label: 'Каталог и партии', icon: CatalogIcon }
    ]
  }

  const tabs = [
    { id: 'analytics', label: 'Аналитика и склад', icon: ChartIcon },
    { id: 'orders', label: 'Продажи', icon: OrdersIcon },
    { id: 'supplies-history', label: 'Журнал поставок', icon: SupplyIcon },
    { id: 'catalog', label: 'Каталог и партии', icon: CatalogIcon },
    { id: 'product', label: 'Новый товар', icon: ProductAddIcon },
    { id: 'writeoff', label: 'Журнал списаний', icon: WriteoffIcon }
  ]

  if (authStore.isAdmin) {
    tabs.push({ id: 'users', label: 'Сотрудники', icon: UsersIcon })
    tabs.push({ id: 'audit', label: 'Аудит и логи', icon: AuditIcon })
  }

  return tabs
})

// Отфильтрованные товары каталога
const filteredCatalogProducts = computed(() => {
  let list = products.value || []

  if (catalogSearch.value.trim()) {
    const q = catalogSearch.value.toLowerCase().trim()
    list = list.filter(p => p.name?.toLowerCase().includes(q))
  }

  if (catalogFilter.value === 'active') {
    list = list.filter(p => p.isActive)
  } else if (catalogFilter.value === 'inStock') {
    list = list.filter(p => p.stock > 0 && p.isActive)
  }

  return list
})

// Формы
const productForm = ref({ name: '', purchasePrice: 120, price: 220, currentQuantity: 15 })
const selectedFile = ref(null)
const imagePreview = ref(null)
const fileInput = ref(null)

const showSupplyModal = ref(false)
const supplyForm = ref({ productId: '', quantity: 25, purchasePrice: 120, sellingPrice: 220 })

// Редактирование цены партии
const editingBatchId = ref(null)
const tempBatchPrice = ref(0)

// Редактирование названия товара
const editingProductNameId = ref(null)
const tempProductName = ref('')

// Модалки пользователей
const showNewUserModal = ref(false)
const newUserForm = ref({ username: '', password: '', roleName: 'Cashier' })
const resetPasswordUser = ref(null)
const newPasswordValue = ref('')

// Автоматическая загрузка данных при смене вкладки
watch(activeTab, (newTab) => {
  if (newTab === 'analytics' && canManageCatalog.value) loadAnalytics()
  if (newTab === 'orders') loadOrders()
  if (newTab === 'supplies-history') loadSupplies()
  if (newTab === 'writeoff') loadWriteOffs()
  if (newTab === 'catalog') loadProducts()
  if (newTab === 'audit' && authStore.isAdmin) loadAuditLogs()
  if (newTab === 'users' && authStore.isAdmin) loadUsers()
})

onMounted(async () => {
  if (canManageCatalog.value) {
    await loadAnalytics()
  }
  await loadProducts()
})

const switchTab = (tabId) => {
  activeTab.value = tabId
}

const selectPeriod = async (p) => {
  selectedPeriod.value = p
  if (p !== 'custom') {
    await loadAnalytics()
  }
}

const getDateParams = (period) => {
  const now = new Date()
  let from = null
  let to = null

  if (period === 'today') {
    from = now.toISOString().split('T')[0]
    to = from
  } else if (period === 'week') {
    const past = new Date()
    past.setDate(past.getDate() - 7)
    from = past.toISOString().split('T')[0]
    to = now.toISOString().split('T')[0]
  } else if (period === 'month') {
    const past = new Date()
    past.setDate(past.getDate() - 30)
    from = past.toISOString().split('T')[0]
    to = now.toISOString().split('T')[0]
  } else if (period === 'custom') {
    from = customDateFrom.value || null
    to = customDateTo.value || null
  }

  return { from, to }
}

const loadAnalytics = async () => {
  loadingAnalytics.value = true
  try {
    const params = getDateParams(selectedPeriod.value)
    const res = await apiClient.get('/analytics/financial-summary', { params })
    summary.value = res.data
  } catch (err) {
    console.error('Ошибка аналитики', err)
  } finally {
    loadingAnalytics.value = false
  }
}

const loadProducts = async () => {
  loadingProducts.value = true
  try {
    const res = await apiClient.get('/products?onlyActive=false')
    products.value = res.data.map(p => ({
      ...p,
      activeBatches: p.activeBatches || []
    }))
  } catch (err) {
    console.error('Ошибка каталога', err)
  } finally {
    loadingProducts.value = false
  }
}

const loadOrders = async () => {
  loadingOrders.value = true
  try {
    const res = await apiClient.get('/orders')
    ordersList.value = res.data
  } catch (err) {
    console.error('Ошибка чеков', err)
  } finally {
    loadingOrders.value = false
  }
}

const loadSupplies = async () => {
  loadingSupplies.value = true
  try {
    const res = await apiClient.get('/supplies')
    suppliesList.value = res.data
  } catch (err) {
    console.error('Ошибка поставок', err)
  } finally {
    loadingSupplies.value = false
  }
}

const loadWriteOffs = async () => {
  loadingWriteOffs.value = true
  try {
    const res = await apiClient.get('/writeoffs')
    writeOffsHistory.value = res.data
  } catch (err) {
    console.error('Ошибка списаний', err)
  } finally {
    loadingWriteOffs.value = false
  }
}

const loadAuditLogs = async () => {
  loadingAudit.value = true
  try {
    const res = await apiClient.get('/audit')
    auditLogs.value = res.data
  } catch (err) {
    console.error('Ошибка логов', err)
  } finally {
    loadingAudit.value = false
  }
}

const loadUsers = async () => {
  loadingUsers.value = true
  try {
    const res = await apiClient.get('/users')
    usersList.value = res.data
  } catch (err) {
    console.error('Ошибка пользователей', err)
  } finally {
    loadingUsers.value = false
  }
}

// Редактирование цены партии
const startEditBatchPrice = (batch) => {
  editingBatchId.value = batch.batchId
  tempBatchPrice.value = batch.sellingPrice
}

const cancelEditBatchPrice = () => {
  editingBatchId.value = null
  tempBatchPrice.value = 0
}

const saveBatchPrice = async (batchId) => {
  if (tempBatchPrice.value < 0) {
    alert('Цена не может быть отрицательной.')
    return
  }

  try {
    await apiClient.patch(`/products/batches/${batchId}/price`, {
      newSellingPrice: tempBatchPrice.value
    })
    editingBatchId.value = null
    await loadProducts()
  } catch (err) {
    alert(err.response?.data?.message || 'Ошибка обновления цены партии')
  }
}

// Редактирование названия товара
const startEditProductName = (product) => {
  editingProductNameId.value = product.id
  tempProductName.value = product.name
}

const cancelEditProductName = () => {
  editingProductNameId.value = null
  tempProductName.value = ''
}

const saveProductName = async (productId) => {
  if (!tempProductName.value.trim()) {
    alert('Название товара не может быть пустым.')
    return
  }

  try {
    await apiClient.patch(`/products/${productId}/name`, {
      name: tempProductName.value.trim()
    })
    editingProductNameId.value = null
    await loadProducts()
  } catch (err) {
    alert(err.response?.data?.message || 'Ошибка изменения названия')
  }
}

// Снятие с продажи или возврат на витрину
const toggleProductStatus = async (product) => {
  if (product.isActive && product.stock > 0) {
    alert(`Нельзя снять товар с продажи, пока на складе есть остаток (${product.stock} шт.).`)
    return
  }

  const actionText = product.isActive ? 'снять с продажи' : 'вернуть в продажу'
  if (!confirm(`Вы действительно хотите ${actionText} товар "${product.name}"?`)) return

  try {
    await apiClient.patch(`/products/${product.id}/toggle-status`)
    await loadProducts()
  } catch (err) {
    alert(err.response?.data?.message || 'Ошибка смены статуса товара')
  }
}

const handleImageSelection = (e) => {
  const file = e.target.files[0]
  if (!file) return
  if (file.size > 5 * 1024 * 1024) {
    alert('Файл больше 5 МБ!')
    resetImage()
    return
  }
  selectedFile.value = file
  imagePreview.value = URL.createObjectURL(file)
}

const resetImage = () => {
  selectedFile.value = null
  imagePreview.value = null
  if (fileInput.value) fileInput.value.value = ''
}

const submitProduct = async () => {
  try {
    isSubmitting.value = true
    let uploadedUrl = null

    if (selectedFile.value) {
      const formData = new FormData()
      formData.append('file', selectedFile.value)
      const uploadRes = await apiClient.post('/products/upload-image', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      })
      uploadedUrl = uploadRes.data.url
    }

    await apiClient.post('/products', {
      name: productForm.value.name.trim(),
      purchasePrice: Number(productForm.value.purchasePrice) || 0,
      price: Number(productForm.value.price) || 0,
      currentQuantity: Number(productForm.value.currentQuantity) || 0,
      imageUrl: uploadedUrl
    })

    alert('Товар успешно создан!')
    productForm.value = { name: '', purchasePrice: 120, price: 220, currentQuantity: 15 }
    resetImage()
    await loadProducts()
    switchTab('catalog')
  } catch (err) {
    alert(err.response?.data?.message || 'Ошибка создания товара')
  } finally {
    isSubmitting.value = false
  }
}

const submitSupply = async () => {
  try {
    isSubmitting.value = true
    await apiClient.post('/supplies', supplyForm.value)
    alert('Партия оприходована!')
    showSupplyModal.value = false
    supplyForm.value = { productId: '', quantity: 25, purchasePrice: 120, sellingPrice: 220 }
    await Promise.all([loadProducts(), loadSupplies()])
  } catch (err) {
    alert(err.response?.data?.message || 'Ошибка оприходования')
  } finally {
    isSubmitting.value = false
  }
}

const submitCreateUser = async () => {
  try {
    await apiClient.post('/users', newUserForm.value)
    alert('Сотрудник успешно создан!')
    showNewUserModal.value = false
    newUserForm.value = { username: '', password: '', roleName: 'Cashier' }
    await loadUsers()
  } catch (err) {
    alert(err.response?.data?.message || 'Ошибка создания пользователя')
  }
}

const toggleUserBlock = async (u) => {
  if (!confirm(`Вы действительно хотите изменить статус пользователя ${u.username}?`)) return
  try {
    await apiClient.patch(`/users/${u.id}/toggle-status`)
    u.isActive = !u.isActive
  } catch (err) {
    alert(err.response?.data?.message || 'Ошибка смены статуса')
  }
}

const openResetPasswordModal = (u) => {
  resetPasswordUser.value = u
  newPasswordValue.value = ''
}

const submitResetPassword = async () => {
  try {
    await apiClient.post(`/users/${resetPasswordUser.value.id}/reset-password`, {
      newPassword: newPasswordValue.value
    })
    alert('Пароль успешно изменен!')
    resetPasswordUser.value = null
  } catch (err) {
    alert(err.response?.data?.message || 'Ошибка сброса пароля')
  }
}

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}


const isOptimizingImages = ref(false)

const triggerBulkImageOptimization = async () => {
  if (!confirm('Запустить процесс оптимизации всех фотографий товаров на сервере?\n\nФайлы будут уменьшены до 600px и переведены в WebP. Это действие необратимо.')) {
    return
  }

  try {
    isOptimizingImages.value = true
    const response = await apiClient.post('/products/optimize-existing-images')
    alert(response.data?.message || 'Оптимизация успешно завершена.')
    // Перезагружаем логи аудита, где появится запись об оптимизации
    await loadAuditLogs()
  } catch (err) {
    alert(err.response?.data?.message || 'Ошибка при оптимизации изображений.')
  } finally {
    isOptimizingImages.value = false
  }
}


</script>

<style scoped>
/* Отключение браузерных стрелок инкремента в числовых полях */
input[type="number"]::-webkit-outer-spin-button,
input[type="number"]::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

input[type="number"] {
  -moz-appearance: textfield;
  appearance: textfield;
}
</style>