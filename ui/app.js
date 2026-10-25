
const micButton = document.getElementById("micButton");
const startButton = document.getElementById("startButton");
const stopButton = document.getElementById("stopButton");
const clearButton = document.getElementById("clearButton");
const saveButton = document.getElementById("saveButton");
const transcript = document.getElementById("transcript");
const wordCount = document.getElementById("wordCount");
const editorStatus = document.getElementById("editorStatus");
const statusPill = document.getElementById("statusPill");
const micTitle = document.getElementById("micTitle");
const micDescription = document.getElementById("micDescription");
const micHalo = document.getElementById("micHalo");
const notice = document.getElementById("notice");
const navDictation = document.getElementById("navDictation");
const navAudio = document.getElementById("navAudio");

let isRecording = false;

function sendToWindows(message) {
    if (window.chrome?.webview) {
        window.chrome.webview.postMessage(message);
    } else {
        showNotice("Bu işlem Windows masaüstü uygulamasında kullanılabilir.");
    }
}

function showNotice(message) {
    notice.textContent = message;
}

function updateWordCount() {
    const text = transcript.value.trim();
    const words = text ? text.split(/\s+/).length : 0;

    wordCount.textContent = `${words} kelime`;
}

function setRecordingState(recording) {
    isRecording = recording;

    micHalo.classList.toggle("recording", recording);
    startButton.disabled = recording;
    micButton.disabled = recording;
    stopButton.disabled = !recording;

    statusPill.innerHTML = recording
        ? '<span class="status-dot"></span> Dinleniyor'
        : '<span class="status-dot"></span> Hazır';

    micTitle.textContent = recording
        ? "Seni dinliyorum"
        : "Konuşmaya başla";

    micDescription.textContent = recording
        ? "Konuşmanı bitirdiğinde durdur düğmesine bas."
        : "Mikrofon düğmesine dokunarak dikteyi başlat.";

    if (recording) {
        showNotice("Dikte başlatma isteği gönderildi.");
    }
}

function startRecording() {
    if (isRecording) return;

    sendToWindows({ action: "startSpeech" });
    setRecordingState(true);
}

function stopRecording() {
    if (!isRecording) return;

    sendToWindows({ action: "stopSpeech" });
    setRecordingState(false);
}

micButton.addEventListener("click", () => {
    if (isRecording) {
        stopRecording();
    } else {
        startRecording();
    }
});

startButton.addEventListener("click", startRecording);
stopButton.addEventListener("click", stopRecording);

clearButton.addEventListener("click", () => {
    if (!transcript.value.trim()) return;

    const confirmed = window.confirm(
        "Metnin tamamını temizlemek istiyor musun?"
    );

    if (!confirmed) return;

    transcript.value = "";
    updateWordCount();
    editorStatus.textContent = "Metin temizlendi";
});

saveButton.addEventListener("click", () => {
    sendToWindows({
        action: "saveText",
        text: transcript.value
    });
});

navAudio.addEventListener("click", () => {
    navAudio.classList.add("active");
    navDictation.classList.remove("active");

    sendToWindows({ action: "selectAudio" });
});

navDictation.addEventListener("click", () => {
    navDictation.classList.add("active");
    navAudio.classList.remove("active");

    showNotice("Sesli yazım alanı seçildi.");
});

transcript.addEventListener("input", () => {
    updateWordCount();
    editorStatus.textContent = "Düzenleniyor";
});

if (window.chrome?.webview) {
    window.chrome.webview.addEventListener("message", (event) => {
        const message = event.data;

        if (!message || typeof message !== "object") return;

        if (message.action === "status") {
            showNotice(message.text || "");
        }

        if (message.action === "transcript") {
            transcript.value += message.text || "";
            updateWordCount();
            editorStatus.textContent = "Metin güncellendi";
        }

        if (message.action === "recordingState") {
            setRecordingState(Boolean(message.recording));
        }
    });
}

updateWordCount();
