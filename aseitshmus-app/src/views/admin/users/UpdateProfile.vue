<script setup>
    import {
        ref,
        onMounted,
        computed
    } from 'vue';
    import {
        useStore
    } from 'vuex';
    import {
        useRouter,
        useRoute
    } from 'vue-router';
    import {
        roles
    } from "../../../constants/RolesConst.js";
    import {
        required
    } from '@vuelidate/validators'
    import useVuelidate from '@vuelidate/core'
    import {
        useToast
    } from 'primevue/usetoast';

    const toast = useToast();
    const route = useRoute();
    const store = useStore()
    const router = useRouter()


    const rules = {
        PersonId: {
            required
        },
        NumberId: {
            required
        },
        FirstName: {
            required
        },
        LastName1: {
            required
        },
        LastName2: {
            required
        },
        DateBirth: {
            required
        },
        RoleDescription: {
            required
        },
        EnrollmentDate: {
            required
        },
        WorkStartDate: {
            required
        },
    }


    const PersonId = ref(route.params.id);
    const roleSelected = ref();
    const statusDB = ref();


    const backLabel = 'Atrás';
    const sendLabel = 'Actualizar';
    const beneficiariesLabel = 'Beneficiarios';
    const activeLabel = 'Activar';
    const inactiveLabel = 'Inactivar';

    const userInfo = ref({
        PersonId: null,
        NumberId: null,
        FirstName: null,
        LastName1: null,
        LastName2: null,
        DateBirth: null,
        RoleId: roleSelected,
        EnrollmentDate: null,
        WorkStartDate: null
    });


    const roleList = ref([{
            name: 'Administrador',
            value: roles.ADMINISTRATOR
        },
        {
            name: 'Presidente',
            value: roles.PRESIDENT
        },
        {
            name: 'Vice-Presidente',
            value: roles.VICEPRESIDENT
        },
        {
            name: 'Asociado',
            value: roles.ASSOCIATE
        }
    ]);

    const status = ref([{
            name: 'Activo',
            value: 1
        },
        {
            name: 'Inactivo',
            value: 0
        }
    ]);

    const manageUserStatus = async () => {
        await store.dispatch('users/patchUserStatus', {
            PersonId: PersonId.value
        })
    }

    const userResponse = computed(() => {
        return store.getters["users/getErrorResponse"];
    });

    const manageUser = async () => {
        await manageUserStatus();
        await fetchUserData();
        if (userResponse.value !== null) {
            toast.add({
                severity: 'error',
                summary: 'Error',
                detail: userResponse.value,
                life: 2000
            });
            store.commit('users/clearErrorResponse');
        } else {       
            if (statusDB.value === 0) {
                toast.add({
                    severity: 'warn',
                    detail: "Usuario ha sido desactivado.",
                    life: 2000
                });              
            } else {
                toast.add({
                    severity: 'success',
                    detail: "Usuario ha sido activado.",
                    life: 2000
                });
            }
        }
    }

    const UserList = () => {
        router.push({
            name: "listUsers"
        });
    }

    const updateBeneficiaries = () => {
        router.push({
            name: "updateBeneficiary",
            params: {
                id: PersonId.value
            },
            props: true,
        });
    };

    const storeUser = async () => {
        await store.dispatch('users/patchUser', {
            PersonId: PersonId.value,
            userInfo: userInfo.value
        })
    }


    const v$ = useVuelidate(rules, userInfo);
    const validateForm = async () => {
        const result = await v$.value.$validate();
        if (!result) {
            if (v$.value.$errors[0].$validator === 'required') {
                toast.add({
                    severity: 'error',
                    detail: 'Por favor revisar los campos en rojo.',
                    life: 2000
                });
            }
            return false
        }
        return true;
    }

    const fetchUserData = async () => {
        await store.dispatch('users/getUserById', {
            rowId: PersonId.value
        });
        const userData = store.getters["users/getUsers"];
        try {
            userInfo.value.PersonId = userData.PersonId;
            userInfo.value.NumberId = userData.NumberId;
            userInfo.value.FirstName = userData.FirstName;
            userInfo.value.LastName1 = userData.LastName1;
            userInfo.value.LastName2 = userData.LastName2;
            userInfo.value.RoleDescription = userData.RoleDescription;
            statusDB.value = userData.IsActive ? 1 : 0;
            userInfo.value.DateBirth = new Date(userData.DateBirth);
            userInfo.value.WorkStartDate = new Date(userData.WorkStartDate);
            roleSelected.value = userData.RoleId;
            if (userData.EnrollmentDate) {
                userInfo.value.EnrollmentDate = new Date(userData.EnrollmentDate);
            } else {
                userInfo.value.EnrollmentDate = null;
            }
        } catch (error) {
            console.error(error);
        }
    };

    const submitData = async (event) => {
        event.preventDefault();
        const isValid = await validateForm();
            if (isValid) {
                try {
                    await storeUser();
                    toast.add({
                        severity: 'success',
                        detail: "Sus cambios han sido guardados.",
                        life: 2000
                    });
                } catch (error) {
                    toast.add({
                        severity: 'error',
                        detail: 'Un error ocurrió.',
                        life: 2000
                    });
                }
            }  
    }

    onMounted(fetchUserData);
</script>

<template>
    <div class="main">
        <toast-component />
        <div class="form">
            <div class="form-row">
                <div class="p-float-label">
                    <input-text class="input-text form-margin-right" id="employee-code" type="text"
                        placeholder="Código de empleado" v-model="userInfo.PersonId"
                        :class="{'p-invalid': v$?.PersonId?.$error }" />
                    <label for="employee-code">Código de empleado</label>
                </div>
                <div class="p-float-label">
                    <input-text class="input-text" id="employee-code" type="text" placeholder="Identificación"
                        v-model="userInfo.NumberId" :class="{'p-invalid': v$?.BankAccount?.$error }" />
                    <label for="employee-code">Identificación</label>
                </div>
            </div>
            <div class="form-row">
                <div class="p-float-label">
                    <input-text placeholder="Nombre" class="input-text form-margin-right" id="employee-firstname"
                        type="text" v-model=" userInfo.FirstName" :class="{'p-invalid': v$?.FirstName?.$error }" />
                    <label for="employee-firstname">Nombre</label>
                </div>
                <div class="p-float-label">
                    <input-text placeholder="Apellido 1" class="dropdown" id="employee-lastname1" type="text"
                        v-model="userInfo.LastName1" :class="{'p-invalid': v$?.LastName1?.$error  }" />
                    <label for="employee-lastname1">Primer apellido</label>
                </div>

            </div>
            <div class="form-row">
                <div class="p-float-label">
                    <input-text placeholder="Apellido 2" class="input-text form-margin-right" id="employee-lastname2"
                        type="text" v-model="userInfo.LastName2" :class="{'p-invalid': v$?.LastName2?.$error }" />
                    <label for="employee-lastname2">Segundo apellido</label>
                </div>
                <drop-down v-model="statusDB" id="status-list" :options="status" optionLabel="name" optionValue="value"
                    placeholder="Estado" class="dropdown" disabled />
            </div>
            <div class="form-row">
                <div class="p-float-label">
                    <date-picker v-model="userInfo.DateBirth" placeholder="Nacimiento"
                        class="dropdown form-margin-right" dateFormat="dd-mm-yy" showIcon id="dob"
                        :class="{'p-invalid': v$?.DateBirth?.$error }" />
                    <label for="dob">Nacimiento</label>
                </div>
                <div class="p-float-label">
                    <date-picker v-model="userInfo.WorkStartDate" placeholder="Ingreso empresa" class="dropdown "
                        dateFormat="dd-mm-yy" showIcon :class="{'p-invalid': v$?.WorkStartDate?.$error}"
                        id="work-start-date" />
                    <label for="work-start-date">Ingreso empresa</label>
                </div>
            </div>
            <div class="form-row">
                <div class="p-float-label">
                    <date-picker v-model="userInfo.EnrollmentDate" placeholder="Ingreso asociación"
                        class="dropdown form-margin-right" dateFormat="dd-mm-yy" showIcon
                        :class="{'p-invalid': v$?.EnrollmentDate?.$error}" id="enrollment-date" />
                    <label for="enrollment-date">Ingreso asociación</label>
                </div>
                <div class="p-float-label">
                    <drop-down v-model="roleSelected" :options="roleList" optionLabel="name" optionValue="value"
                        id="role" placeholder="Rol" class="dropdown" :class="{'p-invalid': v$?.roleSelected?.$error}" />
                    <label for="role">Rol</label>
                </div>
            </div>      
        <div class="actions">
            <base-button class="action-buttons" small :label="backLabel" @click="UserList" :type="'button'" />
            <base-button class="action-buttons" small :label="beneficiariesLabel" @click="updateBeneficiaries" :type="'button'" />
            <base-button class="action-buttons green" small v-if="statusDB === 0" @click="manageUser" :label="activeLabel"
                :type="'submit'" />
            <base-button class="action-buttons red" small v-if="statusDB === 1" @click="manageUser" :label="inactiveLabel"
                :type="'submit'" />
            <base-button class="action-buttons" small :label="sendLabel" @click="submitData" :type="'submit'" />
        </div>
    </div>
    </div>
</template>
<style scoped="scoped">
   .main {
        display: flex;
        justify-content: center;
        align-items: center;
        border: 1px solid #ebebeb;
        border-radius: 5px;
        margin: 1rem;
        padding: 2rem;
    }

    .form {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 100%;
    }

    .form-row {
        display: flex;
        justify-content: space-between;
        align-self: center;
        margin-bottom: 2.4rem;
       
    }

    .form-margin-right {
        margin-right: 6rem;
    }

    .form-margin-left {
        margin-left: 6rem;
    }

    .actions {
        margin-top: 2rem;
        display: flex;
        flex-direction: row;
        justify-content: space-between;
      
    }

    .actions button {
        flex: 1;
        margin-right: 1rem;
    }

    .green,
    .green:hover,
    .green:focus {
        background-color: rgb(6, 100, 6) !important;
        border-color: rgb(6, 100, 6) !important;
    }

    .red,
    .red:hover,
    .green:focus {
        background-color: rgb(189, 90, 90) !important;
        border-color: rgb(189, 90, 90) !important;
    }
</style>