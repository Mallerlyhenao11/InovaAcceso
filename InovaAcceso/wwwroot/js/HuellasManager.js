class HuellasManager {
    constructor() {
        console.log('Inicializando HuellasManager...');
        this.initializeEventListeners();
        this.verificarDispositivo();
    }

    initializeEventListeners() {
        console.log('Inicializando eventos...');

        // Verificar dispositivo
        const btnVerificar = document.getElementById('btnVerificar');

        if (btnVerificar) {
            btnVerificar.addEventListener('click', async () => {
                btnVerificar.disabled = true;
                await this.verificarDispositivo();
                btnVerificar.disabled = false;
            });
        }

        // Capturar huella
        const btnCapturar = document.getElementById('btnCapturar');
        if (btnCapturar) {
            btnCapturar.addEventListener('click', () => {
                this.capturarHuellas(); // ? Esto mantiene `this`
            });
        }

        // Buscar persona
        const btnValidarPersona = document.getElementById('btnValidarPersona');
        if (btnValidarPersona) {
            btnValidarPersona.addEventListener('click', async () => {
                btnVerificar.disabled = true;
                await this.validarPersona();
            });
        }
    }

    async verificarDispositivo() {
        const statusDiv = document.getElementById('deviceStatus');
        try {
            statusDiv.className = 'alert alert-info';
            statusDiv.innerHTML = `
                <i class="fas fa-sync fa-spin me-2"></i>
                Verificando dispositivo...
            `;

            const response = await fetch(API_ENDPOINTS.VERIFICAR_DISPOSITIVO);
            const data = await response.json();

            if (data.success) {
                statusDiv.className = 'alert alert-success';
                statusDiv.innerHTML = `
                    <i class="fas fa-check-circle me-2"></i>
                    ${MENSAJES.DISPOSITIVO_CONECTADO}
                `;
            } else {
                statusDiv.className = 'alert alert-danger';
                statusDiv.innerHTML = `
                    <i class="fas fa-exclamation-circle me-2"></i>
                    ${MENSAJES.DISPOSITIVO_NO_DETECTADO}
                `;
            }

            return data.success;
        } catch (error) {
            console.error('Error al verificar dispositivo:', error);

            statusDiv.className = 'alert alert-danger';
            statusDiv.innerHTML = `
                <i class="fas fa-exclamation-triangle me-2"></i>
                ${MENSAJES.ERROR_VERIFICACION}
            `;

            return false;
        }
    }

    async capturarHuellas() {
        const statusDiv = document.getElementById('deviceStatusHu');
        try {
            statusDiv.className = 'alert alert-info';
            statusDiv.innerHTML = `
            <i class="fas fa-sync fa-spin me-2"></i>
            Capturando huellas...
            `;

            const response = await fetch(API_ENDPOINTS.CAPTURAR, { method: 'POST' });
            const data = await response.json();


            if (data.success) {
                statusDiv.className = 'alert alert-success';
                statusDiv.innerHTML = `
                    <i class="fas fa-check-circle me-2"></i>
                     Huellas Capturadas exitosamente
                `;
                document.getElementById("fingerprintImage1").src = "data:image/png;base64," + data.data;

            } else {
                statusDiv.className = 'alert alert-danger';
                statusDiv.innerHTML = `
                <i class="fas fa-exclamation-circle me-2"></i>
                Intente nuevamente
            `;
            }

            return data.success;
        } catch (error) {
            statusDiv.className = 'alert alert-danger';
            statusDiv.innerHTML = `
            <i class="fas fa-exclamation-triangle me-2"></i>
            Error al procesar la solicitud
        `;
            return false;
        }
    }


    async validarPersona() {
        const isValid = this.validateFields({
            'documentoPersona': MENSAJES.INGRESE_DOC,
        });

        if (!isValid) return;

        const documentoPersona = document.getElementById('documentoPersona').value.trim();
        const deviceStatusPersona = document.getElementById('deviceStatusPersona');
        try {
            deviceStatusPersona.className = 'alert alert-info';
            deviceStatusPersona.innerHTML = `
            <i class="fas fa-sync fa-spin me-2"></i>
            Validando...
            `;

            const response = await fetch(API_ENDPOINTS.VALIDAR_PERSONA(documentoPersona), { method: 'POST' });
            const data = await response.json();


            if (data.success) {
                deviceStatusPersona.className = 'alert alert-success';
                deviceStatusPersona.innerHTML = `
                    <i class="fas fa-check-circle me-2"></i>
                     Empleado encontrado: 
                ` + data.message;

            } else {
                deviceStatusPersona.className = 'alert alert-danger';
                deviceStatusPersona.innerHTML = `
                <i class="fas fa-exclamation-circle me-2"></i>
            ` + data.message;
            }

            return data.success;
        } catch (error) {
            deviceStatusPersona.className = 'alert alert-danger';
            deviceStatusPersona.innerHTML = `
            <i class="fas fa-exclamation-triangle me-2"></i>
            Empledo no encontrado t
        `;
            return false;
        }
    }

    validateFields(fields) {
        for (const [id, message] of Object.entries(fields)) {
            const field = document.getElementById(id);
            if (!field || !field.value.trim()) {
                return false;
            }
        }
        return true;
    }


}
