<template>
  <div class="min-h-screen bg-gray-50">
    <header class="bg-white shadow-sm border-b">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-4">
          <div>
            <h1 class="text-2xl font-bold text-gray-900">Portal do Paciente</h1>
            <p class="text-gray-600">Bem-vindo, {{ usuario?.nome || 'Paciente' }}</p>
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
          <h3 class="text-lg font-semibold text-gray-900 mb-2">Próxima Consulta</h3>
          <p class="text-2xl font-bold text-blue-600">
            {{ proximaConsulta ? formatarDataCurta(proximaConsulta.horario?.dataHoraInicio) : 'Nenhuma' }}
          </p>
        </div>
        <div class="bg-white p-6 rounded-lg shadow">
          <h3 class="text-lg font-semibold text-gray-900 mb-2">Agendamentos Ativos</h3>
          <p class="text-3xl font-bold text-green-600">{{ agendamentosAtivos }}</p>
        </div>
        <div class="bg-white p-6 rounded-lg shadow">
          <h3 class="text-lg font-semibold text-gray-900 mb-2">Médicos Disponíveis</h3>
          <p class="text-3xl font-bold text-purple-600">{{ medicos.length }}</p>
        </div>
      </div>

      <div class="bg-white rounded-lg shadow mb-6">
        <div class="border-b border-gray-200">
          <nav class="flex -mb-px">
            <button
                @click="activeTab = 'agendar'"
                :class="[
                  'py-3 px-6 border-b-2 font-medium text-sm transition-colors',
                  activeTab === 'agendar'
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                ]"
            >
              Agendar Consulta
            </button>
            <button
                @click="activeTab = 'meus-agendamentos'"
                :class="[
                  'py-3 px-6 border-b-2 font-medium text-sm transition-colors',
                  activeTab === 'meus-agendamentos'
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                ]"
            >
              Meus Agendamentos
            </button>
          </nav>
        </div>

        <div class="p-6">
          <div v-if="activeTab === 'agendar'">
            <h2 class="text-xl font-semibold text-gray-900 mb-4">Agendar Nova Consulta</h2>

            <div class="mb-6">
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Selecione o Médico
              </label>
              <div v-if="loadingMedicos" class="text-center py-4">
                <div class="inline-block animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600"></div>
              </div>
              <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                <div
                    v-for="medico in medicos"
                    :key="medico.id"
                    @click="selecionarMedico(medico)"
                    :class="[
                      'border-2 rounded-lg p-4 cursor-pointer transition-all',
                      medicoSelecionado?.id === medico.id
                        ? 'border-blue-500 bg-blue-50'
                        : 'border-gray-200 hover:border-gray-300'
                    ]"
                >
                  <p class="font-semibold text-gray-900">Dr(a). {{ medico.nome }}</p>
                  <p class="text-sm text-gray-600">{{ medico.especialidade }}</p>
                  <p class="text-xs text-gray-500 mt-1">CRM: {{ medico.crm }}</p>
                </div>
              </div>
            </div>

            <div v-if="medicoSelecionado" class="mb-6">
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Horários Disponíveis
              </label>
              <div v-if="loadingHorarios" class="text-center py-4">
                <div class="inline-block animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600"></div>
              </div>
              <div v-else-if="horariosDisponiveis.length === 0" class="text-center py-4 text-gray-500">
                Nenhum horário disponível para este médico
              </div>
              <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
                <div
                    v-for="horario in horariosDisponiveis"
                    :key="horario.id"
                    @click="horarioSelecionado = horario"
                    :class="[
                      'border-2 rounded-lg p-3 cursor-pointer transition-all',
                      horarioSelecionado?.id === horario.id
                        ? 'border-green-500 bg-green-50'
                        : 'border-gray-200 hover:border-gray-300'
                    ]"
                >
                  <p class="font-medium">{{ formatarData(horario.dataHoraInicio) }}</p>
                  <p class="text-sm text-gray-600">
                    {{ formatarHora(horario.dataHoraInicio) }} - {{ formatarHora(horario.dataHoraFim) }}
                  </p>
                </div>
              </div>
            </div>

            <div v-if="horarioSelecionado" class="flex justify-end">
              <button
                  @click="confirmarAgendamento"
                  :disabled="agendando"
                  class="bg-green-600 text-white px-6 py-2 rounded-lg hover:bg-green-700 transition-colors disabled:opacity-50"
              >
                {{ agendando ? 'Agendando...' : 'Confirmar Agendamento' }}
              </button>
            </div>

            <div v-if="errorMessage" class="mt-4 bg-red-50 border border-red-200 text-red-600 px-4 py-3 rounded-lg">
              {{ errorMessage }}
            </div>

            <div v-if="successMessage"
                 class="mt-4 bg-green-50 border border-green-200 text-green-600 px-4 py-3 rounded-lg">
              {{ successMessage }}
            </div>
          </div>

          <div v-if="activeTab === 'meus-agendamentos'">
            <h2 class="text-xl font-semibold text-gray-900 mb-4">Meus Agendamentos</h2>

            <div v-if="loadingAgendamentos" class="text-center py-8">
              <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
              <p class="mt-2 text-gray-600">Carregando agendamentos...</p>
            </div>

            <div v-else-if="agendamentos.length === 0" class="text-center py-8 text-gray-500">
              Você não possui agendamentos
            </div>

            <div v-else class="space-y-4">
              <div
                  v-for="agendamento in agendamentos"
                  :key="agendamento.id"
                  class="border border-gray-200 rounded-lg p-4 hover:bg-gray-50 transition-colors"
              >
                <div class="flex justify-between items-start">
                  <div class="flex-1">
                    <h3 class="font-semibold text-gray-900">
                      Dr(a). {{ getNomeMedicoFromHorario(agendamento.horario) }}
                    </h3>
                    <p class="text-sm text-gray-600 mt-1">
                      Especialidade: {{ agendamento.horario?.medico?.especialidade }}
                    </p>
                    <p class="text-sm text-gray-600">
                      Data: {{ formatarData(agendamento.horario?.dataHoraInicio) }}
                    </p>
                    <p class="text-sm text-gray-600">
                      Horário: {{ formatarHora(agendamento.horario?.dataHoraInicio) }} -
                      {{ formatarHora(agendamento.horario?.dataHoraFim) }}
                    </p>
                    <div class="mt-2">
                      <span :class="getStatusClass(agendamento.status)">
                        {{ agendamento.status }}
                      </span>
                    </div>
                  </div>
                  <button
                      v-if="agendamento.status === 'Agendado'"
                      @click="cancelarAgendamento(agendamento.id)"
                      class="text-red-600 hover:text-red-800 border border-red-300 px-3 py-1 rounded text-sm"
                  >
                    Cancelar
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import {ref, onMounted, computed} from 'vue'
import {useRouter} from 'vue-router'
import {useAuth} from "~/composables/useAuth.js"
import {useRuntimeConfig} from "nuxt/app"

definePageMeta({
  middleware: 'auth'
})

const router = useRouter()
const {getUser, getToken, logout} = useAuth()
const config = useRuntimeConfig()

const usuario = ref(null)
const activeTab = ref('agendar')
const loadingMedicos = ref(false)
const loadingHorarios = ref(false)
const loadingAgendamentos = ref(false)
const agendando = ref(false)

const medicos = ref([])
const medicoSelecionado = ref(null)
const horariosDisponiveis = ref([])
const horarioSelecionado = ref(null)
const agendamentos = ref([])

const errorMessage = ref('')
const successMessage = ref('')

const agendamentosAtivos = computed(() => {
  return agendamentos.value.filter(a => a.status === 'Agendado').length
})

const proximaConsulta = computed(() => {
  const agendamentosOrdenados = agendamentos.value
      .filter(a => a.status === 'Agendado')
      .filter(a => new Date(a.horario?.dataHoraInicio) > new Date())
      .sort((a, b) => new Date(a.horario?.dataHoraInicio) - new Date(b.horario?.dataHoraInicio))

  return agendamentosOrdenados[0] || null
})

onMounted(async () => {
  usuario.value = getUser()
  if (!usuario.value || usuario.value.tipoUsuario !== 'paciente') {
    await router.push('/login')
    return
  }

  await carregarMedicos()
  await carregarAgendamentos()
})

const carregarMedicos = async () => {
  loadingMedicos.value = true
  try {
    const token = getToken()
    const apiBase = config.public.apiBase

    const response = await $fetch(`${apiBase}/api/Medicos`, {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    })

    medicos.value = response || []
  } catch (error) {
    console.error('Erro ao carregar médicos:', error)
    errorMessage.value = 'Erro ao carregar lista de médicos'
  } finally {
    loadingMedicos.value = false
  }
}

const selecionarMedico = async (medico) => {
  medicoSelecionado.value = medico
  horarioSelecionado.value = null
  await carregarHorariosMedico(medico.id)
}

const carregarHorariosMedico = async (medicoId) => {
  loadingHorarios.value = true
  errorMessage.value = ''
  try {
    const token = getToken()
    const apiBase = config.public.apiBase

    const response = await $fetch(`${apiBase}/api/Medicos/${medicoId}/horarios`, {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    })

    horariosDisponiveis.value = (response || []).filter(h =>
        new Date(h.dataHoraFim) >= new Date()
    )
  } catch (error) {
    console.error('Erro ao carregar horários:', error)
    errorMessage.value = 'Erro ao carregar horários disponíveis'
  } finally {
    loadingHorarios.value = false
  }
}

const carregarAgendamentos = async () => {
  loadingAgendamentos.value = true
  try {
    const token = getToken()
    const apiBase = config.public.apiBase

    const response = await $fetch(`${apiBase}/api/Agendamentos`, {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    })

    agendamentos.value = response || []
  } catch (error) {
    console.error('Erro ao carregar agendamentos:', error)
  } finally {
    loadingAgendamentos.value = false
  }
}

const confirmarAgendamento = async () => {
  if (!horarioSelecionado.value) return

  agendando.value = true
  errorMessage.value = ''
  successMessage.value = ''

  try {
    const token = getToken()
    const apiBase = config.public.apiBase

    await $fetch(`${apiBase}/api/Agendamentos`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: {
        horarioId: horarioSelecionado.value.id
      }
    })

    successMessage.value = 'Agendamento realizado com sucesso!'

    medicoSelecionado.value = null
    horarioSelecionado.value = null
    horariosDisponiveis.value = []

    await carregarAgendamentos()

    setTimeout(() => {
      activeTab.value = 'meus-agendamentos'
      successMessage.value = ''
    }, 2000)

  } catch (error) {
    console.error('Erro ao agendar:', error)
    errorMessage.value = error.data?.message || 'Erro ao realizar agendamento'
  } finally {
    agendando.value = false
  }
}

const cancelarAgendamento = async (agendamentoId) => {
  if (!confirm('Tem certeza que deseja cancelar este agendamento?')) return

  try {
    const token = getToken()
    const apiBase = config.public.apiBase

    await $fetch(`${apiBase}/api/Agendamentos/${agendamentoId}/cancelar`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    })

    await carregarAgendamentos()
  } catch (error) {
    console.error('Erro ao cancelar agendamento:', error)
    errorMessage.value = 'Erro ao cancelar agendamento'
  }
}

const handleLogout = () => {
  logout()
  router.push('/login')
}

const formatarData = (dataString) => {
  if (!dataString) return 'N/A'
  const data = new Date(dataString)
  return data.toLocaleDateString('pt-BR')
}

const formatarDataCurta = (dataString) => {
  if (!dataString) return 'N/A'
  const data = new Date(dataString)
  return data.toLocaleDateString('pt-BR', {day: '2-digit', month: '2-digit'})
}

const formatarHora = (dataString) => {
  if (!dataString) return 'N/A'
  const data = new Date(dataString)
  return data.toLocaleTimeString('pt-BR', {hour: '2-digit', minute: '2-digit'})
}

const getStatusClass = (status) => {
  switch (status?.toLowerCase()) {
    case 'agendado':
      return 'inline-block px-2 py-1 bg-green-100 text-green-800 rounded-full text-xs font-medium'
    case 'cancelado':
      return 'inline-block px-2 py-1 bg-red-100 text-red-800 rounded-full text-xs font-medium'
    default:
      return 'inline-block px-2 py-1 bg-gray-100 text-gray-800 rounded-full text-xs font-medium'
  }
}

const getNomeMedicoFromHorario = (horario) => {
  if (!horario?.medico) return ''
  return `${horario.medico.nome}`
}
</script>