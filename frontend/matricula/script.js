
let etapaAtual = 1;
let matriculaId = null;
// === TEMA (Claro/Escuro) ===
const themeSwitch = document.getElementById("themeSwitch");
themeSwitch.addEventListener("change", () => {
  document.body.classList.toggle("light");
});

// ==========================================================
// === CONFIG API ===
const API_URL = "http://localhost:5027/api"; 
// ==========================================================

// === CARREGAR CURSOS AO INICIAR ===
document.addEventListener("DOMContentLoaded", carregarCursos);

async function carregarCursos() {
  try {
    const response = await fetch(`${API_URL}/Curso/GetAllCursosAtivos`);
    const cursos = await response.json();

    preencherCards(cursos);
    preencherSelect(cursos);

  } catch (error) {
    console.error("Erro ao carregar cursos da API:", error);
  }
}

// ==========================================================
// === CRIAR CARDS DINÂMICOS DOS CURSOS ===
function preencherCards(cursos) {
  const container = document.getElementById("coursesContainer");
  container.innerHTML = "";

  cursos.forEach(curso => {
    const card = document.createElement("div");
    card.className = "course-card";

    card.innerHTML = `
      <i class='bx bx-book course-icon'></i>
      <h3>${curso.nome}</h3>
      <p>${curso.descricao}</p>

      <div class="card-buttons">
        <button onclick="showApiDetail('${curso.nome}', '${curso.descricao}')">Saiba mais</button>
      </div>
    `;

    container.appendChild(card);
  });
}

// ==========================================================
// === PREENCHER <SELECT> DO FORM COM CURSOS DA API ===
function preencherSelect(cursos) {
  const select = document.getElementById("cursoSelect");

  select.innerHTML =
    `<option value="" disabled selected>Selecione um curso...</option>`;

  cursos.forEach(curso => {
    const option = document.createElement("option");
    option.value = curso.id;
    option.textContent = curso.nome;
    select.appendChild(option);
  });
}

// ==========================================================
// === MODAL PARA "SAIBA MAIS" ===
function showApiDetail(titulo, descricao) {
  document.getElementById("modalTitle").innerText = titulo;
  document.getElementById("modalText").innerText = descricao;
  document.getElementById("modal").style.display = "flex";
}

function closeModal() {
  document.getElementById("modal").style.display = "none";
}

window.onclick = function (event) {
  const modal = document.getElementById("modal");
  if (event.target === modal) {
    modal.style.display = "none";
  }
};

// ==========================================================
// === IR PARA O FORMULÁRIO AO CLICAR EM "INSCREVA-SE" ===
function goToForm(cursoSelecionado = null) {
  const select = document.getElementById("cursoSelect");

  if (cursoSelecionado) {
    [...select.options].forEach(opt => {
      if (opt.textContent === cursoSelecionado) opt.selected = true;
    });
  }

  document.getElementById("matricula").scrollIntoView({ behavior: "smooth" });
}

// ==========================================================
// === FORMATAÇÃO CPF ===
const cpfInput = document.getElementById("cpf");

cpfInput.addEventListener("input", () => {
  let value = cpfInput.value.replace(/\D/g, "");

  if (value.length > 3) value = value.replace(/(\d{3})(\d)/, "$1.$2");
  if (value.length > 7) value = value.replace(/(\d{3})(\d)/, "$1.$2");
  if (value.length > 11) value = value.replace(/(\d{3})(\d)/, "$1-$2");

  cpfInput.value = value;
});

// ==========================================================
// === ENVIO DO FORM ===
document.getElementById("formMatricula").addEventListener("submit", (e) => {
  e.preventDefault();
  alert("Matrícula realizada com sucesso! 🎉");
});



// Mostrar etapas do wizard
function mostrarEtapa(n) {
  document.querySelectorAll(".wizard-step").forEach(step => step.classList.remove("active"));
  document.querySelector(`#step${n}`).classList.add("active");

  // progresso
  const steps = document.querySelectorAll(".progress-bar .step");
  steps.forEach((s, i) => {
    s.classList.toggle("active", i < n);
  });
}

// Avançar
async function nextStep() {
  if (etapaAtual === 1) {
    await enviarEtapa1();
    return;
  }

  if (etapaAtual === 2) {
    await enviarEtapa2();
    return;
  }
}

// Voltar
function prevStep() {
  etapaAtual--;
  mostrarEtapa(etapaAtual);
}

// --- Funções do modal de sucesso ---
function abrirModalSucesso() {
  const modal = document.getElementById("modalSucesso");
  const btnFechar = document.getElementById("btnFecharModal");
  if (!modal) return;

  modal.style.display = "flex";

  if (btnFechar) btnFechar.focus();

  function escListener(e) {
    if (e.key === "Escape") fechar();
  }
  document.addEventListener("keydown", escListener);

  function clickForaListener(e) {
    if (e.target === modal) fechar();
  }
  modal.addEventListener("click", clickForaListener);

  function fechar() {
    modal.style.display = "none";
    document.removeEventListener("keydown", escListener);
    modal.removeEventListener("click", clickForaListener);
  }

  if (btnFechar) {
    btnFechar.onclick = fechar;
  }
}

function resetarFormulario() {
  etapaAtual = 1;
  mostrarEtapa(etapaAtual);

  matriculaId = null;

  document.getElementById("nome").value = "";
  document.getElementById("email").value = "";
  document.getElementById("cpf").value = "";

  const select = document.getElementById("cursoSelect");
  select.selectedIndex = 0;

  document.getElementById("comprovante").value = "";
  document.getElementById("documento_historico").value = "";
  document.getElementById("documento_cpf").value = "";

  document.querySelectorAll(".progress-bar .step").forEach((step, index) => {
    step.classList.toggle("active", index === 0);
  });

  document.getElementById("step1").scrollIntoView({ behavior: "smooth" });
}


function mostrarErro(mensagem) {
  document.getElementById("modalErroMensagem").textContent = mensagem;

  const modal = document.getElementById("modalErro");
  modal.style.display = "flex";
}

function fecharModalErro() {
  const modal = document.getElementById("modalErro");
  modal.style.display = "none";
}


async function enviarEtapa1() {
  const nome = document.getElementById("nome").value.trim();
  const email = document.getElementById("email").value.trim();
  const cpf = document.getElementById("cpf").value.trim();
  const curso = document.getElementById("cursoSelect").value;

  


  if (!nome || !email || !cpf || !curso) {
    alert("Preencha todos os dados pessoais.");
    return;
  }

  try {
    const response = await fetch(`${API_URL}/Matricula`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        nome,
        email,
        cpf,
        cursoId: parseInt(curso)
      })
    });

    const data = await response.json();

    if (!response.ok) throw data;

    matriculaId = data.matriculaId;

    etapaAtual = 2;
    mostrarEtapa(etapaAtual);

  } catch (err) {
    console.error("Erro etapa 1:", err);
    mostrarErro(err?.erro );
  }
}


async function enviarEtapa2() {
  const comprovante = document.getElementById("comprovante");

  if (!comprovante.files.length) {
    alert("Envie o comprovante de pagamento.");
    return;
  }

  const formData = new FormData();
  formData.append("arquivo", comprovante.files[0]);

  try {
    const response = await fetch(`${API_URL}/Matricula/${matriculaId}/pagamento`, {
      method: "POST",
      body: formData
    });

    const data = await response.json();
    if (!response.ok) throw data;

    etapaAtual = 3;
    mostrarEtapa(etapaAtual);

  } catch (err) {
    console.error("Erro etapa 2:", err);
    alert("Erro ao enviar comprovante.");
  }
}


async function finalizarMatricula() {
  const documento_historico = document.getElementById("documento_historico");
  const documento_cpf = document.getElementById("documento_cpf");

  if (!documento_historico.files.length || !documento_cpf.files.length) {
    alert("Envie os documentos finais.");
    return;
  }

  const formData = new FormData();
  formData.append("documentoHistorico", documento_historico.files[0]);
  formData.append("documentoCpf", documento_cpf.files[0]);

  try {
    const response = await fetch(`${API_URL}/Matricula/${matriculaId}/documentos`, {
      method: "POST",
      body: formData
    });

    const data = await response.json();

    if (!response.ok) throw data;

    abrirModalSucesso();
    resetarFormulario();
  } catch (err) {
    console.error("Erro etapa 3:", err);
    alert("Erro ao enviar documentos.");
  }
}
