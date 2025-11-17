(function(){
  emailjs.init("uXff3S5w8KeovuO4C");
})();

// ===== Alternar entre seções =====
const menuItems = document.querySelectorAll(".sidebar nav ul li");
const dashboardButtons = document.querySelectorAll(".dashboard-btn");
const sections = document.querySelectorAll("main section");

function showSection(sectionId) {
  menuItems.forEach(i => i.classList.remove("active"));
  sections.forEach(sec => sec.classList.remove("active"));
  document.getElementById(sectionId).classList.add("active");

  const menuItem = [...menuItems].find(i => i.dataset.section === sectionId);
  if (menuItem) menuItem.classList.add("active");
}

// Clicar no menu lateral
menuItems.forEach(item => {
  item.addEventListener("click", () => showSection(item.dataset.section));
});

// Clicar nos botões do dashboard
dashboardButtons.forEach(btn => {
  btn.addEventListener("click", () => showSection(btn.dataset.section));
});


// ===== Alternar tema claro/escuro =====
const themeSwitch = document.getElementById("themeSwitch");
themeSwitch.addEventListener("change", () => {
  document.body.classList.toggle("light", themeSwitch.checked);
});

// ===== Cadastro e listagem =====
const form = document.getElementById("cadastroForm");
const tabelaBody = document.getElementById("tabelaBody");
const filtro = document.getElementById("filtro");

// ===== Mostrar campo de matrícula se for aluno =====
const tipoSelect = document.getElementById("tipo");
const matriculaGroup = document.getElementById("matriculaGroup");

tipoSelect.addEventListener("change", () => {
  if (tipoSelect.value === "aluno") {
    matriculaGroup.classList.remove("hidden");
    matriculaGroup.querySelector("input").setAttribute("required", "true");
  } else {
    matriculaGroup.classList.add("hidden");
    matriculaGroup.querySelector("input").removeAttribute("required");
  }
});


//confirma cadastro
form.addEventListener("submit", e => {
  e.preventDefault();

  const tipo = document.getElementById("tipo").value;
  const nome = document.getElementById("nome").value.trim();
  const email = document.getElementById("email").value.trim();
  const senha = document.getElementById("senha").value.trim();
  const curso = document.getElementById("curso").value.trim();

  if (!tipo || !nome || !email || !senha || !curso) {
    alert("Preencha todos os campos!");
    return;
  }

  if (!email.includes("@")) {
    alert("E-mail inválido!");
    return;
  }

  const params = { tipo, nome, email, senha, curso };

  // temporario enquanto ainda nao tem o back
  const row = document.createElement("tr");
  row.innerHTML = `<td>${tipo}</td><td>${nome}</td><td>${email}</td><td>${curso}</td>`;
  tabelaBody.appendChild(row);

  const botao = form.querySelector(".btn");
  botao.disabled = true;
  botao.textContent = "Enviando...";

  emailjs.send("tivit_academy", "template_zdwi0zn", params)
    .then(() => {
      form.reset();
    })
    .catch((error) => {
      console.error("Erro ao enviar e-mail:", error);
    })
    .finally(() => {
      botao.disabled = false;
      botao.textContent = "Cadastrar";
    });
});

// ===== Filtro =====
filtro.addEventListener("input", () => {
  const valor = filtro.value.toLowerCase();
  const linhas = tabelaBody.querySelectorAll("tr");

  linhas.forEach(linha => {
    const nome = linha.children[1].textContent.toLowerCase();
    linha.style.display = nome.includes(valor) ? "" : "none";
  });
});

// ===== Mostrar/Ocultar Senha =====
const toggleSenha = document.getElementById('toggleSenha');
const inputSenha = document.getElementById('senha');

toggleSenha.addEventListener('click', () => {
  const isPassword = inputSenha.type === 'password';
  inputSenha.type = isPassword ? 'text' : 'password';
  toggleSenha.classList.toggle('bx-show', isPassword);
  toggleSenha.classList.toggle('bx-hide', !isPassword);
});


// === POPUP DE NOVO CURSO ===
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


// === POPUP NOVA TURMA ===
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
