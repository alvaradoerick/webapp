<script setup>
    import {
        ref,
        onMounted,
        computed,
        watch
    } from 'vue';
    import {
        useStore
    } from 'vuex';
    import {
        useRouter
    } from 'vue-router';
    import {
        useToast
    } from 'primevue/usetoast';

    const router = useRouter();
    const store = useStore()
    const toast = useToast();

    const requestData = ref([]);
    const backLabel = 'Principal';
    const deletionStatus = ref(false);

    const fetchRequestData = async () => {
        await store.dispatch('savingsRequests/getAllSavings');
        const requests = store.getters['savingsRequests/getSavings'];
        requestData.value = requests.map(request => {
            return {
                ...request,
                IsActive: request.IsActive ? "Activo" : "Inactivo",
                IsApproved: request.IsApproved  ? "Activo"  : request.IsApproved === null  ? "Pendiente"  : "Rechazado",
                ApplicationDate: new Date(request.ApplicationDate).toLocaleString("es-ES", dateFormat),
            };
        });
    };

    const storeRequest = async (id) => {
        await store.dispatch('savingsRequests/deleteSavings', {
            rowId: id
        })
    }

    const deleteResponse = computed(() => {
        return store.getters["savingsRequests/getErrorResponse"];
    });

    const deleteRecord = async (rowData) => {
        try {
            await storeRequest(rowData.data.SavingsRequestId);
            if (deleteResponse.value === null) {
                toast.add({
                    severity: 'warn',
                    detail: "Solicitud de ahorro ha sido eliminada.",
                    life: 2000
                });
                deletionStatus.value = true;
            } else {
                toast.add({
                    severity: 'error',
                    detail: deleteResponse.value,
                    life: 3000
                });
                store.commit('savingsRequests/clearErrorResponse');
            }
        } catch (error) {
            toast.add({
                severity: 'error',
                detail: error,
                life: 2000
            });
        }
    };
    const dateFormat = {
        day: "numeric",
        month: "numeric",
        year: "numeric"
    };
    watch(deletionStatus, (newStatus) => {
        if (newStatus) {
            fetchRequestData();
            deletionStatus.value = false;
        }
    });

    const cancel = () => {
        router.push({
            name: "dashboard"
        });
    }
    
    const updateRecord = (rowData) => {
        router.push({
            name: "updateSavingRequest",
            params: {
                id: rowData.data.SavingsRequestId
            },
            props: true,
        });
    };

    onMounted(fetchRequestData);
</script>

<template>
    <toast-component />
    <div class="list">
        <data-table :value="requestData" paginator :rows="3" tableStyle="min-width: 80rem">
            <data-column field="Name" header="Nombre" sortable></data-column>
            <data-column field="SavingsTypeName" header="Tipo de ahorro" sortable></data-column>
            <data-column field="ApplicationDate" header="Fecha de solicitud" sortable></data-column>         
            <data-column field="IsActive" header="Estado" sortable style="width: 160px"></data-column>
            <data-column field="IsApproved" header="Estado de aprobación" sortable></data-column>
            <data-column header="" style="width: 100px"> <template #body="rowData">
                    <base-button class="action-buttons" label="Editar" @click="updateRecord(rowData)"
                        :type="'button'" />
                </template></data-column>
            <data-column header="" style="width: 100px"> <template #body="rowData">
                    <base-button class="action-buttons" label="Eliminar" @click="deleteRecord(rowData)"
                        :type="'button'" />
                </template></data-column>
        </data-table>

        <div class="actions-container">
            <div class="actions">
                <base-button :label="backLabel" @click="cancel" :type="'button'" />
            </div>
        </div>
    </div>
</template>

<style scoped="scoped">
    .list {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 100%;
    }

    .action-buttons {
        display: flex;
        overflow: hidden;
        width: 105px;
        color: white;
        text-align: center;
        flex-direction: column;
        align-items: center;
    }

    .actions-container {
        position: static;
        bottom: 0;
        background-color: #fff;
        width: 100%;
        justify-content: center;
    }

    .actions {
        display: flex;
        flex: 1;
        align-items: center;
        margin-top: 2rem;
        justify-content: space-between;
    }
</style>