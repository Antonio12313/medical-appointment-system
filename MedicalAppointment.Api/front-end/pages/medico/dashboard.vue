<template>
  <div class="min-h-screen bg-gray-50">
    <header class="bg-white shadow-sm border-b">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-4">
          <div>
            <h1 class="text-2xl font-bold text-gray-900">Dashboard Médico</h1>
            <p class="text-gray-600">Bem-vindo, {{ usuario?.nome || 'Médico' }}</p>
          </div>
          <button
              @click="handleLogout"
              class="bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 transition-colors"
          >
            Sair
          </button>
        </div>
      </div>
    </header>

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div class="bg-white p-6 rounded-lg shadow">
          <h3 class="text-lg font-semibold text-gray-900 mb-2">Horários Disponíveis</h3>
          <p class="text-3xl font-bold text-blue-600">{{ horariosDisponiveisCount }}</p>
        </div>
        <div class="bg-white p-6 rounded-lg shadow">
          <h3 class="text-lg font-semibold text-gray-900 mb-2">Agendamentos Hoje</h3>
          <p class="text-3xl font-bold text-green-600">{{ agendamentosHoje }}</p>
        </div>
        <div class="bg-white p-6 rounded-lg shadow">
          <h3 class="text-lg font-semibold text-gray-900 mb-2">Total de Pacientes</h3>
          <p class="text-3xl font-bold text-purple-600">{{ totalPacientes }}</p>
        </div>
      </div>

      <div class="bg-white rounded-lg shadow p-6 mb-8">
        <h2 class="text-xl font-semibold text-gray-900 mb-4">Ações Rápidas</h2>
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          <button
              @click="showCreateHorario = true"
              class="p-4 bg-blue-50 rounded-lg border border-blue-200 hover:bg-blue-100 transition-colors"
          >
            <div class="text-blue-600 font-semibold">Criar Horário</div>
            <div class="text-sm text-blue-500">Disponibilizar novo horário</div>
          </button>
          <button
              @click="loadData"
              class="p-4 bg-green-50 rounded-lg border border-green-200 hover:bg-green-100 transition-colors"
          >
            <div class="text-green-600 font-semibold">Ver Agendamentos</div>
            <div class="text-sm text-green-500">Consultas marcadas</div>
          </button>
          <button
              @click="loadMeusHorarios"
              class="p-4 bg-purple-50 rounded-lg border border-purple-200 hover:bg-purple-100 transition-colors"
          >
            <div class="text-purple-600 font-semibold">Meus Horários</div>
            <div class="text-sm text-purple-500">Gerenciar disponibilidade</div>
          </button>
        </div>
      </div>

      <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-xl font-semibold text-gray-900 mb-4">Agendamentos Recentes</h2>
        <div v-if="loading" class="text-center py-8">
          <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
          <p class="mt-2 text-gray-600">Carregando...</p>
        </div>
        <div v-else-if="agendamentos.length === 0" class="text-center py-8 text-gray-500">
          Nenhum agendamento encontrado
        </div>
        <div v-else class="space-y-4">
          <div v-for="agendamento in agendamentos" :key="agendamento.id"
               class="border border-gray-200 rounded-lg p-4 hover:bg-gray-50 transition-colors">
            <div class="flex justify-between items-start">
              <div>
                <h3 class="font-semibold text-gray-900">Paciente: {{ agendamento.paciente.nome }}</h3>
                <p class="text-sm text-gray-600">{{ formatarData(agendamento.horario?.dataHoraInicio) }}</p>
                <p class="text-sm text-gray-500">Status: {{ agendamento.status }}</p>
              </div>
              <span :class="getStatusClass(agendamento.status)">
                {{ agendamento.status }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <div v-if="showMeusHorarios" class="bg-white rounded-lg shadow p-6 mt-8">
        <div class="flex justify-between items-center mb-4">
          <h2 class="text-xl font-semibold text-gray-900">Meus Horários</h2>
          <button
              @click="showMeusHorarios = false"
              class="text-gray-400 hover:text-gray-600"
          >
            ✕
          </button>
        </div>

        <div v-if="loadingHorarios" class="text-center py-8">
          <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
          <p class="mt-2 text-gray-600">Carregando horários...</p>
        </div>

        <div v-else-if="meusHorarios.length === 0" class="text-center py-8 text-gray-500">
          Você não tem horários cadastrados
        </div>

        <div v-else class="space-y-3">
          <div v-for="horario in meusHorarios" :key="horario.id"
               class="border border-gray-200 rounded-lg p-3 hover:bg-gray-50 transition-colors">
            <div class="flex justify-between items-center">
              <div>
                <p class="font-semibold">
                  {{ formatarData(horario.dataHoraInicio) }} - {{ formatarHora(horario.dataHoraFim) }}
                </p>
                <p class="text-sm" :class="horario.disponivel ? 'text-green-600' : 'text-red-600'">
                  {{ horario.disponivel ? 'Disponível' : 'Ocupado' }}
                </p>
              </div>
              <button
                  v-if="horario.disponivel"
                  @click="deletarHorario(horario.id)"
                  class="text-red-600 hover:text-red-800 border border-red-300 px-3 py-1 rounded text-sm"
              >
                Excluir
              </button>
            </div>
          </div>
        </div>
      </div>
    </main>

    <div v-if="showCreateHorario"
         class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="bg-white rounded-lg p-6 w-full max-w-md">
        <h3 class="text-lg font-semibold mb-4">Criar Novo Horário</h3>
        <form @submit.prevent="criarHorario" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Data e Hora de Início</label>
            <input
                v-model="novoHorario.dataHoraInicio"
                type="datetime-local"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Data e Hora de Fim</label>
            <input
                v-model="novoHorario.dataHoraFim"
                type="datetime-local"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>

          <div v-if="errorHorario" class="bg-red-50 border border-red-200 text-red-600 px-3 py-2 rounded text-sm">
            {{ errorHorario }}
          </div>

          <div class="flex space-x-3">
            <button
                type="submit"
                :disabled="creatingHorario"
                class="flex-1 bg-blue-600 text-white py-2 px-4 rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50"
            >
              {{ creatingHorario ? 'Criando...' : 'Criar' }}
            </button>
            <button
                type="button"
                @click="showCreateHorario = false"
                class="flex-1 bg-gray-300 text-gray-700 py-2 px-4 rounded-lg hover:bg-gray-400 transition-colors"
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import {useRuntimeConfig} from "nuxt/app";

definePageMeta({
  middleware: 'auth'
})

import {ref, onMounted, computed} from 'vue'
import {useRouter} from 'vue-router'
import {useAuth} from "~/composables/useAuth.js";

const router = useRouter()
const {getUser, getToken, logout} = useAuth()
const config = useRuntimeConfig()

const usuario = ref(null)
const loading = ref(true)
const loadingHorarios = ref(false)
const agendamentos = ref([])
const meusHorarios = ref([])
const showCreateHorario = ref(false)
const showMeusHorarios = ref(false)
const creatingHorario = ref(false)
const errorHorario = ref('')

const novoHorario = ref({
  dataHoraInicio: '',
  dataHoraFim: ''
})

const horariosDisponiveisCount = computed(() => {
  return meusHorarios.value.filter(h => h.disponivel).length
})

const agendamentosHoje = computed(() => {
  const hoje = new Date().toDateString()
  return agendamentos.value.filter(a =>
      new Date(a.horario?.dataHoraInicio).toDateString() === hoje
  ).length
})

const totalPacientes = computed(() => {
  return new Set(agendamentos.value.map(a => a.pacienteId)).size
})

onMounted(async () => {
  usuario.value = getUser()
  if (!usuario.value || usuario.value.tipoUsuario !== 'medico') {
    await router.push('/login')
    return
  }

  await loadData()
  await loadMeusHorarios()
})

const loadData = async () => {
  loading.value = true
  try {
    const token = getToken()
    const apiBase = config.public.apiBase

    const agendamentosRes = await $fetch(`${apiBase}/api/Agendamentos`, {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    })

    agendamentos.value = agendamentosRes || []

  } catch (error) {
    console.error('Erro ao carregar dados:', error)
  } finally {
    loading.value = false
  }
}

const loadMeusHorarios = async () => {
  loadingHorarios.value = true
  showMeusHorarios.value = true
  try {
    const token = getToken()
    const apiBase = config.public.apiBase

    const horarios = await $fetch(`${apiBase}/api/Horarios/meus-horarios`, {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    })

    meusHorarios.value = horarios || []

  } catch (error) {
    console.error('Erro ao carregar horários:', error)
  } finally {
    loadingHorarios.value = false
  }
}

const criarHorario = async () => {
  creatingHorario.value = true
  errorHorario.value = ''

  try {
    const token = getToken()
    const apiBase = config.public.apiBase

    await $fetch(`${apiBase}/api/Horarios`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: {
        dataHoraInicio: new Date(novoHorario.value.dataHoraInicio).toISOString(),
        dataHoraFim: new Date(novoHorario.value.dataHoraFim).toISOString()
      }
    })

    showCreateHorario.value = false
    novoHorario.value = {dataHoraInicio: '', dataHoraFim: ''}
    await loadMeusHorarios()

  } catch (error) {
    console.error('Erro ao criar horário:', error)
    errorHorario.value = error.data?.message || 'Erro ao criar horário'
  } finally {
    creatingHorario.value = false
  }
}

const deletarHorario = async (horarioId) => {
  if (!confirm('Tem certeza que deseja excluir este horário?')) return

  try {
    const token = getToken()
    const apiBase = config.public.apiBase

    await $fetch(`${apiBase}/api/Horarios/${horarioId}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    })

    await loadMeusHorarios()

  } catch (error) {
    console.error('Erro ao excluir horário:', error)
  }
}

const handleLogout = () => {
  logout()
  router.push('/login')
}

const formatarData = (dataString) => {
  if (!dataString) return 'N/A'
  return new Date(dataString).toLocaleString('pt-BR')
}

const formatarHora = (dataString) => {
  if (!dataString) return 'N/A'
  return new Date(dataString).toLocaleTimeString('pt-BR', {
    hour: '2-digit',
    minute: '2-digit'
  })
}

const getStatusClass = (status) => {
  switch (status?.toLowerCase()) {
    case 'agendado':
      return 'px-2 py-1 bg-green-100 text-green-800 rounded-full text-xs'
    case 'cancelado':
      return 'px-2 py-1 bg-red-100 text-red-800 rounded-full text-xs'
    default:
      return 'px-2 py-1 bg-gray-100 text-gray-800 rounded-full text-xs'
  }
}
</script>