// Configuración y constantes
console.log("El archivo huellas.js se ha cargado correctamente.");
const API_ENDPOINTS = {
    VERIFICAR_DISPOSITIVO: '/api/HuellaApi/verificar-dispositivo',
    CAPTURAR: '/api/HuellaApi/capturar-huella',
    VALIDAR_PERSONA: (documentoNumero) => `/api/HuellaApi/verificar-persona/${documentoNumero}`,
    ENROLAR: `/api/HuellaApi/enrolar-huella`,
};

const MENSAJES = {
    DISPOSITIVO_CONECTADO: 'Dispositivo conectado y listo',
    DISPOSITIVO_NO_DETECTADO: 'Dispositivo no detectado',
    ERROR_VERIFICACION: 'Error al verificar el dispositivo',
    TIMEOUT_VERIFICACION: 'Tiempo de espera agotado al verificar el dispositivo',
    INGRESE_DOC: 'Por favor, ingrese el Documento de la persona',
    INGRESE_RESPONSABLE: 'Por favor, ingrese el responsable',
    CONFIRMAR_DESACTIVACION: '¿Está seguro de desactivar esta huella?',
    NO_REGISTROS: 'No se encontraron registros',
    ERROR_GENERICO: 'Error al procesar la solicitud'
};

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
            btnCapturar.addEventListener('click',  () => {
                this.capturarHuellas(); // ✅ Esto mantiene `this`
            });
        }

        // Buscar persona
        const btnValidarPersona = document.getElementById('btnValidarPersona');
        if (btnValidarPersona) {
            btnValidarPersona.addEventListener('click', async () => {
                await this.validarPersona();
            });
        }

        // Enrolar huella 
        const btnEnrolarHuellas = document.getElementById('btnEnrolarHuellas');
        if (btnEnrolarHuellas) {
            btnEnrolarHuellas.addEventListener('click', async () => {
                await this.enrolarHuellas();
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
            console.error('Error al verificar dispositivo:', error.message);

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


            if (data.success)
            {
                statusDiv.className = 'alert alert-success';
                statusDiv.innerHTML = `
                    <i class="fas fa-check-circle me-2"></i>
                     Huella Capturada exitosamente
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
        const documentoPersona = document.getElementById('documentoPersona').value.trim();
        const deviceStatusPersona = document.getElementById('deviceStatusPersona');

        // Validar que el campo no esté vacío y contenga solo números
        if (!documentoPersona || isNaN(documentoPersona)) {
            deviceStatusPersona.className = 'alert alert-warning';
            deviceStatusPersona.innerHTML = `
            <i class="fas fa-exclamation-circle me-2"></i>
            Ingrese un número de documento válido.
        `;
            return false;
        }

        try {
            deviceStatusPersona.className = 'alert alert-info';
            deviceStatusPersona.innerHTML = `
            <i class="fas fa-sync fa-spin me-2"></i>
            Validando...
        `;

            const documentoNumero = parseInt(documentoPersona, 10);
            const url = API_ENDPOINTS.VALIDAR_PERSONA(documentoNumero);
            console.log("URL generada:", url);
            const response = await fetch(url, { method: 'GET' });

            if (!response.ok) {
                throw new Error(`Error en la validación: ${response.status} ${response.statusText}`);
            }

            const data = await response.json();

            if (data.success) {
                deviceStatusPersona.className = 'alert alert-success';
                deviceStatusPersona.innerHTML = `
                <i class="fas fa-check-circle me-2"></i>
                Empleado encontrado: ${data.message}
            `;
            } else {
                deviceStatusPersona.className = 'alert alert-danger';
                deviceStatusPersona.innerHTML = `
                <i class="fas fa-exclamation-circle me-2"></i>
                ${data.message}
            `;
            }

            return data.success;
        } catch (error) {
            console.error("Error en la validación:", error);
            deviceStatusPersona.className = 'alert alert-danger';
            deviceStatusPersona.innerHTML = `
            <i class="fas fa-exclamation-triangle me-2"></i>
            Empleado no encontrado. Verifique el número e intente nuevamente.
        `;
            return false;
        }
    }

    async  enrolarHuellas() {
    const documentoPersona = document.getElementById('documentoPersona').value.trim();
    const src = document.getElementById("fingerprintImage1").src;

    if (src.startsWith("data:image/png;base64,")) {
        try {
            // Extraer la cadena Base64
            const base64String = src.split(",")[1];

            // Convertir Base64 a array de bytes
            const byteCharacters = atob(base64String);
            const byteNumbers = new Array(byteCharacters.length);

            for (let i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }

            const dataToSend = {
                documentoPersona: documentoPersona,
                imagenBytes: byteNumbers // Array de bytes
            };

            // Validar datos antes de enviarlos
            if (!documentoPersona) {
                alert("El documento de la persona es obligatorio.");
                return;
            }

            if (!dataToSend.imagenBytes || dataToSend.imagenBytes.length === 0) {
                alert("La imagen de la huella es inválida o está vacía.");
                return;
            }

            console.log("Datos enviados al servidor:", dataToSend);

            // Enviar solicitud al servidor
            const response = await fetch(API_ENDPOINTS.ENROLAR, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(dataToSend)
            });

            if (response.ok) {
                const responseData = await response.json();
                console.log("Huella enrolada con éxito:", responseData);
                alert("Huella enrolada con éxito.");
            } else {
                const errorData = await response.json();
                console.error("Error al enrolar la huella:", errorData);
                alert(`Error: ${errorData.title || "No se pudo enrolar la huella."}\nDetalles: ${JSON.stringify(errorData.errors)}`);
            }
        } catch (error) {
            console.error("Error durante el proceso de enrolamiento:", error);
            alert("Ocurrió un error inesperado. Por favor, inténtalo de nuevo.");
        }
    } else {
        alert("La imagen de la huella no es válida. Asegúrate de que sea una imagen en formato PNG codificada en Base64.");
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
// Inicialización
document.addEventListener('DOMContentLoaded', () => {
    new HuellasManager();
});
