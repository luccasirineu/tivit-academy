(function () {
  emailjs.init("uXff3S5w8KeovuO4C");
})();

/* ============================
   CONTROLE DE SEÇÕES
============================ */
const menuItems = document.querySelectorAll(".sidebar nav ul li");
const dashboardButtons = document.querySelectorAll(".dashboard-btn");
const sections = document.querySelectorAll("main section");

function showSection(sectionId) {
  menuItems.forEach(i => i.classList.remove("active"));
  sections.forEach(sec => sec.classList.remove("active"));
  document.getElementById(sectionId).classList.add("active");

  const menuItem = [...menuItems].find(i => i.dataset.section === sectionId);
  if (menuItem) menuItem.classList.add("active");

  // Quando entrar na seção de matrículas → carregar lista
  if (sectionId === "gerenciarMatriculas") {
    carregarMatriculasPendentes();
  }
}

menuItems.forEach(item => {
  item.addEventListener("click", () => showSection(item.dataset.section));
});
dashboardButtons.forEach(btn => {
  btn.addEventListener("click", () => showSection(btn.dataset.section));
});

/* ============================
   TEMA DARK/LIGHT
============================ */
const themeSwitch = document.getElementById("themeSwitch");
themeSwitch.addEventListener("change", () => {
  document.body.classList.toggle("light", themeSwitch.checked);
});

/* ============================
   MODAL DE ERRO
============================ */
function abrirModalErro(msg) {
  const modal = document.getElementById("modalErro");
  const modalMsg = document.getElementById("modalErroMsg");
  modalMsg.textContent = msg;
  modal.classList.remove("hidden");
}

function fecharModalErro() {
  document.getElementById("modalErro").classList.add("hidden");
}

/* ============================
   LISTAR MATRÍCULAS PENDENTES
============================ */
const API_MATRICULAS = "http://localhost:5027/api/Matricula/getAllMatriculas";

async function carregarMatriculasPendentes() {
  const lista = document.getElementById("listaMatriculas");
  lista.innerHTML = "<p>Carregando...</p>";

  try {
    const response = await fetch(API_MATRICULAS);
    const dados = await response.json(); 

    if (!response.ok) {
      throw dados;
    }


    const pendentes = dados.filter(m =>
      m.status?.toUpperCase() === "AGUARDANDO_APROVACAO"
    );

    if (pendentes.length === 0) {
      lista.innerHTML = "<p>Nenhuma matrícula pendente.</p>";
      return;
    }

    lista.innerHTML = "";

    pendentes.forEach(m => {
      const card = document.createElement("div");
      card.classList.add("matricula-card");

      card.innerHTML = `
        <h3>${m.nome}</h3>
        <div class="matricula-info">
          <p><strong>E-mail:</strong> ${m.email}</p>
          <p><strong>CPF:</strong> ${m.cpf}</p>
          <p><strong>Curso :</strong> ${m.cursoId}</p>
        </div>

        <div class="card-actions">
          <button class="btn-aprovar" onclick="aprovar('${m.cpf}')">Aprovar</button>
          <button class="btn-reprovar" onclick="reprovar('${m.cpf}')">Reprovar</button>
        </div>
      `;

      lista.appendChild(card);
    });

  } catch (err) {
    console.error("Erro:", err);
    abrirModalErro("Erro ao carregar solicitações.");
  }
}


/* ============================
   APROVAR / REPROVAR
============================ */
async function aprovar(id) {
  try {
    const response = await fetch(`http://localhost:5027/api/Matricula/aprovar/${id}`, {
      method: "POST"
    });

    if (!response.ok) throw await response.json();

    carregarMatriculasPendentes();

  } catch (err) {
    console.error(err);
    abrirModalErro("Erro ao aprovar matrícula.");
  }
}

async function reprovar(id) {
  try {
    const response = await fetch(`http://localhost:5027/api/Matricula/reprovar/${id}`, {
      method: "POST"
    });

    if (!response.ok) throw await response.json();

    carregarMatriculasPendentes();

  } catch (err) {
    console.error(err);
    abrirModalErro("Erro ao reprovar matrícula.");
  }
}

/* ============================
   POPUPS DE CURSO E TURMA 
   (sem alterações)
============================ */
const btnNovoCurso = document.getElementById("btnNovoCurso");
const popupNovaCursos = document.getElementById("popupNovaCursos");
const btnFecharCurso = document.getElementById("btnFecharPopup");
const formNovaCursos = document.getElementById("formNovaCursos");

btnNovoCurso.addEventListener("click", () => {
  popupNovaCursos.classList.remove("hidden");
});

btnFecharCurso.addEventListener("click", () => {
  popupNovaCursos.classList.add("hidden");
});

formNovaCursos.addEventListener("submit", (e) => {
  e.preventDefault();
  alert("Novo curso cadastrado com sucesso! (mock)");
  formNovaCursos.reset();
  popupNovaCursos.classList.add("hidden");
});

const btnNovaTurma = document.getElementById("btnNovaTurma");
const popupNovaTurma = document.getElementById("popupNovaTurma");
const fecharPopupTurma = document.getElementById("fecharPopup");
const formTurma = document.getElementById("formTurma");
const listaTurmas = document.getElementById("listaTurmas");

btnNovaTurma.addEventListener("click", () => {
  popupNovaTurma.classList.remove("hidden");
});

fecharPopupTurma.addEventListener("click", () => {
  popupNovaTurma.classList.add("hidden");
});

popupNovaTurma.addEventListener("click", (e) => {
  if (e.target === popupNovaTurma) {
    popupNovaTurma.classList.add("hidden");
  }
});

formTurma.addEventListener("submit", (e) => {
  e.preventDefault();

  const nome = document.getElementById("nomeTurma").value;
  const curso = document.getElementById("cursoTurma").value;
  const prof = document.getElementById("professorTurma").value;
  const turno = document.getElementById("turnoTurma").value;
  const alunos = document.getElementById("alunosTurma").value;
  const status = document.getElementById("statusTurma").value;

  const card = document.createElement("div");
  card.classList.add("turma-item");
  card.innerHTML = `
    <div class="turma-info">
      <h3>${nome}</h3>
      <p><i class='bx bx-book'></i> Curso: ${curso}</p>
      <p><i class='bx bx-user'></i> Professor: ${prof}</p>
      <p><i class='bx bx-group'></i> ${alunos} alunos • Turno: ${turno}</p>
    </div>
    <div class="turma-status ${status}">${status === "ativo" ? "Ativa" : "Concluída"}</div>
    <div class="turma-actions">
      <button class="btn-edit"><i class='bx bx-edit-alt'></i></button>
      <button class="btn-delete"><i class='bx bx-trash'></i></button>
    </div>
  `;

  listaTurmas.appendChild(card);
  formTurma.reset();
  popupNovaTurma.classList.add("hidden");
});

/* ============================
   SIDEBAR
============================ */
const toggleBtn = document.getElementById("toggleSidebar");
const sidebar = document.querySelector(".sidebar");
const body = document.body;
const floatingBtn = document.getElementById("sidebarToggleFloating");

function toggleSidebar() {
  sidebar.classList.toggle("collapsed");
  body.classList.toggle("sidebar-collapsed");
}

if (toggleBtn) toggleBtn.addEventListener("click", toggleSidebar);
if (floatingBtn) floatingBtn.addEventListener("click", toggleSidebar);
