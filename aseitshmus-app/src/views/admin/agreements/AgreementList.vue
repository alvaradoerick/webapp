<script setup>
    import DataTable from 'primevue/datatable';
    import Column from 'primevue/column';
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

    const backLabel = 'Principal';
    const addLabel = 'Agregar';
    const agreementData = ref([]);
    const deletionStatus = ref(false);

 

    const fetchAgreementData = async () => {
        await store.dispatch('agreements/getAllAgreements');
        const agreements = store.getters['agreements/getAgreement'];
        agreementData.value = agreements.map(agreement => {
            return {
                ...agreement,
                IsActive: agreement.IsActive ? "Activo" : "Inactivo"
            };         
        });
    };

    const storeAgreement = async (id) => {
        await store.dispatch('agreements/deleteAgreement', {
            rowId: id
        })
    }

    const deleteResponse = computed(() => {
        return store.getters["agreements/getErrorResponse"];
    });

    const deleteRecord = async (rowData) => {
        console.log(rowData)
        try {
            await storeAgreement(rowData.data.AgreementId);
            if (deleteResponse.value === null) {
                toast.add({
                    severity: 'warn',
                    detail: "El convenio ha sido eliminado.",
                    life: 2000
                });
                deletionStatus.value = true;
            } else {
                toast.add({
                    severity: 'error',
                    detail: deleteResponse.value,
                    life: 3000
                });
                store.commit('agreements/clearErrorResponse');
            }
        } catch (error) {
            console.log(error)
            toast.add({
                severity: 'error',
                detail: `Un error ocurrió. ${error}`,
                life: 2000
            });
        }
    };

    watch(deletionStatus, (newStatus) => {
        if (newStatus) {
            fetchAgreementData();
            deletionStatus.value = false;
        }
    });


    const goBack = () => {
        router.push({
            name: "dashboard"
        });
    }

    const addAgreement = () => {
        router.push({
            name: "createAgreement"
        });
    }
    const updateCategory = (rowData) => {
        router.push({
            name: "updateAgreement",
            params: {
                id: rowData.data.AgreementId
            },
            props: true,
        });
    };


    onMounted(fetchAgreementData);
 
    
</script>

<template>
            <toast-component />
    <div class="agreement-list">

        <DataTable :value="agreementData"  tableStyle="min-width: 80rem" paginator :rows="3">
            <Column field="Title" header="Convenio" sortable></Column>
            <Column field="CategoryName" header="Categoría" sortable style="width: 300px"></Column>
            <Column field="IsActive" header="Estado" sortable style="width: 180px"></Column>
            <Column header="" style="width: 100px"> <template #body="rowData">
                    <base-button class="action-buttons" label="Editar" :type="'button'" @click="updateCategory(rowData)"/>
                </template></Column>
            <Column header="" style="width: 100px"> <template #body="rowData">
                    <base-button class="action-buttons" label="Eliminar"  @click="deleteRecord(rowData)" :type="'button'" />
                </template></Column>
        </DataTable>
    <div class="actions-container">
        <div class="actions">
            <base-button :label="backLabel" @click="goBack" :type="'button'" />
            <base-button :label="addLabel" @click="addAgreement" :type="'button'" />
        </div>
    </div>
</div>
</template>


<style scoped="scoped">
   .agreement-list {
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