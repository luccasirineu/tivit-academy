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

  if (sectionId === "professores") {
      carregarProfessores();
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

async function getCursoNome(cursoId) {
  try {
    const API_CURSOS = "http://localhost:5027/api/Curso";

    const response = await fetch(`${API_CURSOS}/${cursoId}`);
    const curso = await response.json();

    if (!response.ok) throw curso;
    return curso.nome; 
  } catch (err) {
    console.error("Erro ao buscar curso:", err);
    return "Nome indisponível";
  }
}

/* ============================
   LISTAR MATRÍCULAS PENDENTES
============================ */
const API_MATRICULAS = "http://localhost:5027/api/Matricula/getAllMatriculasPendentes";

async function carregarMatriculasPendentes() {
  const lista = document.getElementById("listaMatriculas");
  lista.innerHTML = "<p>Carregando...</p>";

  try {
    const response = await fetch(API_MATRICULAS);
    const dados = await response.json(); 

    if (!response.ok) {
      throw dados;
    }

    if (dados.length === 0) {
      lista.innerHTML = "<p>Nenhuma matrícula pendente.</p>";
      return;
    }

    lista.innerHTML = "";

    for (const m of dados) {

      const cursoNome = await getCursoNome(m.cursoId);

      const card = document.createElement("div");
      card.classList.add("matricula-card");

      card.innerHTML = `
        <h3>${m.nome}</h3>
        <div class="matricula-info">
          <p><strong>E-mail:</strong> ${m.email}</p>
          <p><strong>CPF:</strong> ${m.cpf}</p>
          <p><strong>Curso:</strong> ${cursoNome}</p>
        </div>

        <div class="card-actions">
          <button class="btn-aprovar" onclick="aprovar('${m.id}')">Aprovar</button>
          <button class="btn-reprovar" onclick="reprovar('${m.id}')">Reprovar</button>
        </div>
      `;

      lista.appendChild(card);
    }

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
    const response = await fetch(`http://localhost:5027/api/Matricula/recusar/${id}`, {
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


const API_BASE = "http://localhost:5027/api";

async function carregarResumoAdmin() {
  try {
    const usuarioLogado = JSON.parse(localStorage.getItem("usuarioLogado"));

    if (!usuarioLogado || !usuarioLogado.id) {
      console.warn("Administrador não identificado");
      return;
    }


    await Promise.all([
      carregarQntdAlunosAtivos(),
      carregarQntdProfessoresAtivos(),
      carregarQtdTurmas()
    ]);

  } catch (error) {
    console.error("Erro ao carregar resumo:", error);
  }
}


async function carregarQntdAlunosAtivos() {
  const response = await fetch(
    `${API_BASE}/Aluno/getQntdAlunosAtivos`
  );

  if (!response.ok) throw new Error("Erro ao buscar alunos ativos");

  const qtdAlunos = await response.json();

  document.getElementById("qtdAlunosAtivos").textContent = qtdAlunos;
}

async function carregarQntdProfessoresAtivos() {
  const response = await fetch(
    `${API_BASE}/Professor/getQntdProfessoresAtivos`
  );

  if (!response.ok) throw new Error("Erro ao buscar turmas");

  const qtdProfessor = await response.json();
  document.getElementById("qtdProfessoresAtivos").textContent = qtdProfessor;
}

async function carregarQtdTurmas() {
  const response = await fetch(
    `${API_BASE}/Turma/getQntdTurmasAtivas`
  );

  if (!response.ok) throw new Error("Erro ao buscar eventos");

  const eventos = await response.json();

  document.getElementById("qtdTurmas").textContent = eventos;
}

document.addEventListener("DOMContentLoaded", carregarResumoAdmin);


/* ============================
   Busca usuario
============================ */

document.getElementById("btnBuscarAluno").addEventListener("click", buscarBoletimAluno);

async function buscarBoletimAluno() {
  const valorBusca = document.getElementById("searchAluno").value.trim();

  if (!valorBusca) {
    return;
  }

  let url = "";

  // Verifica se é matrícula (somente números)
  if (/^\d+$/.test(valorBusca)) {
    url = `${API_BASE}/User/getUserByCpf/?cpf=${valorBusca}`;
  } else {
    url = `${API_BASE}/User/getUsersByNome/?nome=${encodeURIComponent(valorBusca)}`;
  }

  try {
    const response = await fetch(url);
    if (!response.ok) throw new Error("Aluno não encontrado");

    const dados = await response.json();

    if (!dados || dados.length === 0) {
      return;
    }

    renderizarDados(dados);

  } catch (error) {
    console.error(error);
  }
}
function renderizarDados(dadosApi) {
  const resultadoBox = document.getElementById("resultadoAluno");
  const listaUsuarios = document.getElementById("listaUsuarios");

  const usuarios = normalizarParaLista(dadosApi);

  resultadoBox.classList.remove("hidden");
  listaUsuarios.innerHTML = "";

  if (usuarios.length === 0) {
    listaUsuarios.innerHTML = "<p>Nenhum usuário encontrado.</p>";
    return;
  }

  usuarios.forEach(usuario => {
  const div = document.createElement("div");
  div.classList.add("usuario-card");

  const isAtivo = usuario.status === "ATIVO";

  const icone = isAtivo ? "bx-user-x" : "bx-user-check";
  const titulo = isAtivo ? "Desativar usuário" : "Ativar usuário";
  const acao = isAtivo ? "desativar" : "ativar";

  div.innerHTML = `
    <div class="usuario-header">
      <p><strong>Nome:</strong> ${usuario.nome}</p>

    </div>

    <p><strong>Email:</strong> ${usuario.email}</p>
    <p><strong>CPF:</strong> ${usuario.cpf || "Não informado"}</p>
    <p><strong>Tipo:</strong> ${usuario.tipo || "Não informado"}</p>
    <p><strong>Status:</strong> ${usuario.status || "Não informado"}</p>
    <i class='bx ${icone} btn-acao-usuario'
            title="${titulo}"
            data-cpf="${usuario.cpf}"
            data-tipo="${usuario.tipo}"
            data-acao="${acao}">
          </i>
    <hr />
  `;

  listaUsuarios.appendChild(div);
});

  // Ativa eventos após renderizar
  ativarEventosDesativar();
}

function ativarEventosDesativar() {
  document.querySelectorAll(".btn-acao-usuario").forEach(btn => {
    btn.addEventListener("click", async (event) => {
      const botao = event.currentTarget;
      const { cpf, tipo, acao } = botao.dataset;

      if (!cpf || !tipo || !acao) {
        abrirModal({
          titulo: "Erro",
          mensagem: "Dados do usuário inválidos.",
          confirmarTexto: "OK",
          somenteOk: true
        });
        return;
      }

      const mensagem =
        acao === "desativar"
          ? "Tem certeza que deseja desativar este usuário?"
          : "Tem certeza que deseja ativar este usuário?";

      abrirModal({
        titulo: "Confirmação",
        mensagem,
        confirmarTexto: acao === "desativar" ? "Desativar" : "Ativar",
        onConfirm: async () => {
          if (acao === "desativar") {
            await desativarUsuario(cpf, tipo, botao);
          } else {
            await ativarUsuario(cpf, tipo, botao);
          }
        }
      });
    });
  });
}




async function desativarUsuario(cpf, tipo, botao) {
  try {
    const response = await fetch(
      `${API_BASE}/User/desativar?cpf=${cpf}&tipo=${tipo}`,
      { method: "PUT" }
    );

    if (!response.ok)
      throw new Error();

    atualizarIconeUsuario(botao, "DESATIVADO");

    abrirModal({
      titulo: "Sucesso",
      mensagem: "Usuário desativado com sucesso!",
      confirmarTexto: "OK",
      somenteOk: true
    });

  } catch (error) {
    console.error(error);
    abrirModal({
      titulo: "Erro",
      mensagem: "Não foi possível desativar o usuário.",
      confirmarTexto: "OK",
      somenteOk: true
    });
  }
}



async function ativarUsuario(cpf, tipo, botao) {
  try {
    const response = await fetch(
      `${API_BASE}/User/ativar?cpf=${cpf}&tipo=${tipo}`,
      { method: "PUT" }
    );

    if (!response.ok)
      throw new Error();

    atualizarIconeUsuario(botao, "ATIVO");

    abrirModal({
      titulo: "Sucesso",
      mensagem: "Usuário ativado com sucesso!",
      confirmarTexto: "OK",
      somenteOk: true
    });

  } catch (error) {
    console.error(error);
    abrirModal({
      titulo: "Erro",
      mensagem: "Não foi possível ativar o usuário.",
      confirmarTexto: "OK",
      somenteOk: true
    });
  }
}




function normalizarParaLista(dados) {
  if (!dados) return [];

  // Se já for lista
  if (Array.isArray(dados)) {
    return dados;
  }

  // Se for objeto único
  return [dados];
}


function atualizarIconeUsuario(botao, novoStatus) {
  const card = botao.closest(".usuario-card");

  if (novoStatus === "DESATIVADO") {
    botao.classList.remove("bx-user-x");
    botao.classList.add("bx-user-check");

    botao.dataset.acao = "ativar";
    botao.title = "Ativar usuário";
  } else {
    botao.classList.remove("bx-user-check");
    botao.classList.add("bx-user-x");

    botao.dataset.acao = "desativar";
    botao.title = "Desativar usuário";
  }

  // Atualiza o texto de status
  const statusParagrafo = [...card.querySelectorAll("p")]
    .find(p => p.textContent.includes("Status:"));

  if (statusParagrafo) {
    statusParagrafo.innerHTML = `<strong>Status:</strong> ${novoStatus}`;
  }
}

function abrirModal({
  titulo,
  mensagem,
  confirmarTexto = "Confirmar",
  onConfirm = null,
  somenteOk = false
}) {
  const modal = document.getElementById("modalConfirmacao");
  const tituloEl = document.getElementById("modalTitulo");
  const msgEl = document.getElementById("modalMensagem");
  const btnConfirmar = document.getElementById("btnConfirmar");
  const btnCancelar = document.getElementById("btnCancelar");

  tituloEl.textContent = titulo;
  msgEl.textContent = mensagem;
  btnConfirmar.textContent = confirmarTexto;

  modal.classList.remove("hidden");

  btnCancelar.style.display = somenteOk ? "none" : "inline-block";

  const fechar = () => {
    modal.classList.add("hidden");
    btnConfirmar.onclick = null;
    btnCancelar.onclick = null;
  };

  btnCancelar.onclick = fechar;

  btnConfirmar.onclick = async () => {
    if (onConfirm) await onConfirm();
    fechar();
  };
}

const cursosGrid = document.getElementById("cursosGrid");
const btnNovoCurso = document.getElementById("btnNovoCurso");
const popupNovoCurso = document.getElementById("popupNovoCurso");
const selectProfessorCurso = document.getElementById("selectProfessorCurso");
const btnSalvarCurso = document.getElementById("btnSalvarCurso");
const btnCancelarCurso = document.getElementById("btnCancelarCurso");
const inputNomeCurso = document.getElementById("nomeNovoCurso");
const inputDescricaoCurso = document.getElementById("descricaoNovoCurso");
let cursoParaExcluirId = null;

// ========== EVENTOS ==========
btnNovoCurso.addEventListener("click", () => {
  popupNovoCurso.classList.remove("hidden");
  carregarProfessoresAtivos();
});

btnSalvarCurso.addEventListener("click", salvarNovoCurso);
btnCancelarCurso.addEventListener("click", fecharPopupCurso);

// ========== FUNÇÕES ==========
async function carregarCursos() {
  try {
    cursosGrid.innerHTML = "";
    const response = await fetch("http://localhost:5027/api/Curso");
    if (!response.ok) throw new Error("Erro ao buscar cursos");

    const cursos = await response.json();

    for (const curso of cursos) {
      const professorNome = await buscarProfessorNome(curso.profResponsavel);
      const qtdAlunos = await buscarQtdAlunosPorCurso(curso.id);
      criarCardCurso(
        curso.id,
        curso.nome,
        professorNome,
        qtdAlunos,
        curso.descricao,
        curso.status
      );
    }
  } catch (error) {
    console.error(error);
    cursosGrid.innerHTML = "<p>Erro ao carregar cursos</p>";
  }
}

async function buscarProfessorNome(professorId) {
  try {
    const response = await fetch(
      `http://localhost:5027/api/Professor/getProfessorById/${professorId}`
    );
    if (!response.ok) throw new Error();
    const professor = await response.json();
    return professor.nome;
  } catch {
    return "Professor não encontrado";
  }
}

async function buscarQtdAlunosPorCurso(cursoId) {
  try {
    const response = await fetch(
      `http://localhost:5027/api/Curso/getQntdAlunosByCursoId/${cursoId}`
    );
    if (!response.ok) throw new Error();
    const quantidade = await response.json();
    return quantidade;
  } catch {
    return 0;
  }
}

function criarCardCurso(cursoId, nomeCurso, nomeProfessor, qtdAlunos, descricao, status) {
  const card = document.createElement("div");
  card.classList.add("cursos-card");

  const isAtivo = status === "ATIVO";

  card.innerHTML = `
    <h3>${nomeCurso}</h3>
    <p><i class='bx bx-user'></i> Professor: ${nomeProfessor}</p>
    <p><i class='bx bx-group'></i> Alunos: ${qtdAlunos}</p>
    <p><i class='bx bx-radio-circle'></i> Status: ${status}</p>

    <div class="card-actions">
      <button class="btn-edit">
        <i class='bx bx-edit'></i>
      </button>

      <button class="btn-toggle-status">
        <i class='bx ${isAtivo ? "bx-x-circle" : "bx-check-circle"}'></i>
      </button>
    </div>
  `;

  cursosGrid.appendChild(card);
  // Editar
  card.querySelector(".btn-edit").addEventListener("click", () => {
    abrirPopupEdicao({
      id: cursoId,
      nome: nomeCurso,
      descricao,
      nomeProfessor
    });
  });

  // Ativar / Desativar
  card.querySelector(".btn-toggle-status").addEventListener("click", () => {
    cursoParaExcluirId = cursoId;

    if (status === "ATIVO") {
      abrirPopupConfirmacao(
        "Desativar Curso",
        "Tem certeza que deseja desativar este curso?",
        () => desativarCurso(cursoId)
      );
    } else {
      abrirPopupConfirmacao(
        "Ativar Curso",
        "Deseja ativar este curso novamente?",
        () => ativarCurso(cursoId)
      );
    }
  });
}

async function desativarCurso(cursoId) {
  try {
    const response = await fetch(
      `http://localhost:5027/api/Curso/desativarCurso/${cursoId}`,
      { method: "PUT" }
    );

    if (!response.ok) throw new Error();

    fecharPopupConfirmacao();
    abrirModal("Sucesso", "Curso desativado com sucesso!");
    carregarCursos();

  } catch (error) {
    console.error(error);
    abrirModal("Erro", "Erro ao desativar curso");
  }
}

async function ativarCurso(cursoId) {
  try {
    const response = await fetch(
      `http://localhost:5027/api/Curso/ativarCurso/${cursoId}`,
      { method: "PUT" }
    );

    if (!response.ok) throw new Error();

    fecharPopupConfirmacao();
    abrirModal("Sucesso", "Curso ativado com sucesso!");
    carregarCursos();

  } catch (error) {
    console.error(error);
    abrirModal("Erro", "Erro ao ativar curso");
  }
}



let acaoConfirmacao = null;

function abrirPopupConfirmacao(titulo, mensagem, callback) {
  document.getElementById("tituloConfirmacao").textContent = titulo;
  document.getElementById("mensagemConfirmacao").textContent = mensagem;

  acaoConfirmacao = callback;

  document
    .getElementById("popupExcluirCurso")
    .classList.remove("hidden");
}

function fecharPopupConfirmacao() {
  document
    .getElementById("popupExcluirCurso")
    .classList.add("hidden");

  acaoConfirmacao = null;
}

document
  .getElementById("btnConfirmarExclusao")
  .addEventListener("click", () => {
    if (acaoConfirmacao) acaoConfirmacao();
  });

document
  .getElementById("btnCancelarExclusao")
  .addEventListener("click", fecharPopupConfirmacao);


let cursoEmEdicaoId = null;

function abrirPopupEdicao(curso) {
  cursoEmEdicaoId = curso.id;

  document.getElementById("editNomeCurso").value = curso.nome;
  document.getElementById("editDescricaoCurso").value = curso.descricao || "";

  carregarProfessoresEdicao(curso.nomeProfessor);

  document
    .getElementById("popupEditarCurso")
    .classList.remove("hidden");
}


async function carregarProfessoresEdicao(nomeProfessorAtual) {
  const select = document.getElementById("editSelectProfessor");
  select.innerHTML = "";

  const response = await fetch(
    "http://localhost:5027/api/Professor/getAllProfessores"
  );
  const professores = await response.json();

  professores.forEach(prof => {
    const option = document.createElement("option");
    option.value = prof.id;
    option.textContent = prof.nome;

    if (prof.nome === nomeProfessorAtual) {
      option.selected = true;
    }

    select.appendChild(option);
  });
}

async function carregarProfessoresAtivos() {
  try {
    // Limpa select
    selectProfessorCurso.innerHTML = 
      `<option value="">Carregando professores...</option>`;

    const response = await fetch(
      "http://localhost:5027/api/Professor/getAllProfessoresAtivos"
    );

    if (!response.ok) {
      throw new Error("Erro ao buscar professores");
    }

    const professores = await response.json();

    // Limpa novamente antes de preencher
    selectProfessorCurso.innerHTML =
      `<option value="">Selecione um professor</option>`;

    professores.forEach(prof => {
      const option = document.createElement("option");
      option.value = prof.id;
      option.textContent = prof.nome;
      selectProfessorCurso.appendChild(option);
    });

  } catch (error) {
    console.error(error);

    selectProfessorCurso.innerHTML =
      `<option value="">Erro ao carregar professores</option>`;
  }
}


async function salvarNovoCurso() {
  const nomeCurso = inputNomeCurso.value.trim();
  const professorId = Number(selectProfessorCurso.value);
  const descricaoCurso = inputDescricaoCurso.value;
  if (!nomeCurso || !professorId || !inputDescricaoCurso) {
    
    return;
  }

  const payload = {
    nome: nomeCurso,
    descricao : descricaoCurso,
    profResponsavel: professorId
  };

  btnSalvarCurso.disabled = true;
  btnSalvarCurso.textContent = "Salvando...";

  try {
    const response = await fetch("http://localhost:5027/api/Curso/criarCurso", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    if (!response.ok) {
      const erro = await response.text();
      throw new Error(erro || "Erro ao criar curso");
    }

    fecharPopupCurso();
    carregarCursos();

  } catch (error) {
    console.error(error);
  } finally {
    btnSalvarCurso.disabled = false;
    btnSalvarCurso.textContent = "Salvar";
  }
}

function fecharPopupCurso() {
  popupNovoCurso.classList.add("hidden");
  inputNomeCurso.value = "";
  selectProfessorCurso.value = "";
}

document
  .getElementById("btnSalvarEdicao")
  .addEventListener("click", async () => {

    const nome = document.getElementById("editNomeCurso").value.trim();
    const descricao = document.getElementById("editDescricaoCurso").value.trim();
    const professorId = Number(
      document.getElementById("editSelectProfessor").value
    );

    if (!nome || !professorId) {
      abrirModal("Atenção", "Preencha todos os campos");
      return;
    }

    const payload = {
      id: cursoEmEdicaoId,
      nome,
      descricao,
      profResponsavel: professorId
    };

    try {
      const response = await fetch(
        `http://localhost:5027/api/Curso/atualizarCurso`,
        {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(payload)
        }
      );

      if (!response.ok) throw new Error();

      abrirModal("Sucesso", "Curso atualizado com sucesso!");
      document
        .getElementById("popupEditarCurso")
        .classList.add("hidden");

      carregarCursos();

    } catch (error) {
      console.error(error);
      abrirModal("Erro", "Erro ao atualizar curso");
    }
  });
document
  .getElementById("btnCancelarEdicao")
  .addEventListener("click", () => {
    document
      .getElementById("popupEditarCurso")
      .classList.add("hidden");

    cursoEmEdicaoId = null;
  });


// Inicializar
carregarCursos();


function criarCardTurma(turmaId, nomeTurma, nomeCurso, cursoId, status) {
  const card = document.createElement("div");
  card.classList.add("turmas-card");

  card.innerHTML = `
    <div class="turma-info">
      <h3>Turma: ${nomeTurma}</h3>
      <p><i class='bx bx-book'></i> Curso: ${nomeCurso}</p>
    </div>

    <div class="turma-status ${status.toLowerCase()}">
      ${status}
    </div>

    <div class="turma-actions">
      <button 
        class="btn-edit"
        data-turma-id="${turmaId}"
        data-curso-id="${cursoId}"
        data-nome="${nomeTurma}"
        data-status="${status}"
      >
        <i class='bx bx-edit-alt'></i>
      </button>
    
    </div>
  `;

  turmasGrid.appendChild(card);
}



async function buscarCurso(cursoId) {
  try {
    const response = await fetch(
      `http://localhost:5027/api/Curso/${cursoId}`
    );
    if (!response.ok) throw new Error();
    const curso = await response.json();
    return curso;
  } catch (error) {
    console.error(error);
}
}

async function carregarTurmas() {
  try {
    turmasGrid.innerHTML = "";

    const [turmasRes, cursosRes] = await Promise.all([
      fetch("http://localhost:5027/api/Turma/getAllTurmas"),
      fetch("http://localhost:5027/api/Curso")
    ]);

    if (!turmasRes.ok || !cursosRes.ok) throw new Error();

    const turmas = await turmasRes.json();
    const cursos = await cursosRes.json();

    for (const turma of turmas) {
      const curso = cursos.find(c => c.id === turma.cursoId);

      criarCardTurma(
        turma.id,
        turma.nome,
        curso ? curso.nome : "Curso não encontrado",
        turma.cursoId,
        turma.status
      );
    }

  } catch (error) {
    console.error(error);
    turmasGrid.innerHTML = "<p>Erro ao carregar turmas</p>";
  }
}


carregarTurmas();

const popupEditarTurma = document.getElementById("popupEditarTurma");
const editNomeTurma = document.getElementById("editNomeTurma");
const editSelectCurso = document.getElementById("editSelectCurso");
const editStatus = document.getElementById("editStatus");
const btnCancelarEdicaoTurma = document.getElementById("btnCancelarEdicaoTurma");
const btnSalvarEdicaoTurma = document.getElementById("btnSalvarEdicaoTurma");

let turmaEditandoId = null;

const btnNovaTurma = document.getElementById("btnNovaTurma");
const popupNovaTurma = document.getElementById("popupNovaTurma");
const formTurma = document.getElementById("formTurma");
const cursoTurmaSelect = document.getElementById("cursoTurma");
const fecharPopup = document.getElementById("fecharPopup");


btnNovaTurma.addEventListener("click", async () => {
  popupNovaTurma.classList.remove("hidden");
  await carregarCursosAtivos();
});


async function carregarCursosAtivos() {
  try {
    cursoTurmaSelect.innerHTML = `<option value="">Carregando...</option>`;

    const response = await fetch("http://localhost:5027/api/Curso");
    if (!response.ok) throw new Error();

    const cursos = await response.json();

    const cursosAtivos = cursos.filter(c => c.status === "ATIVO");

    cursoTurmaSelect.innerHTML = `<option value="">Selecione um curso</option>`;

    cursosAtivos.forEach(curso => {
      const option = document.createElement("option");
      option.value = curso.id;
      option.textContent = curso.nome;
      cursoTurmaSelect.appendChild(option);
    });

  } catch (error) {
    console.error(error);
    cursoTurmaSelect.innerHTML = `<option value="">Erro ao carregar</option>`;
  }
}

fecharPopup.addEventListener("click", () => {
  popupNovaTurma.classList.add("hidden");
  formTurma.reset();
});


formTurma.addEventListener("submit", async (e) => {
  e.preventDefault();

  const payload = {
    nome: document.getElementById("nomeTurma").value.trim(),
    cursoId: Number(cursoTurmaSelect.value),
    status: document.getElementById("statusTurma").value.toUpperCase()
  };

  if (!payload.nome || !payload.cursoId) {
    alert("Preencha todos os campos obrigatórios.");
    return;
  }

  try {
    const response = await fetch("http://localhost:5027/api/Turma/criarTurma", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    if (!response.ok) throw new Error();

    alert("Turma criada com sucesso!");

    popupNovaTurma.classList.add("hidden");
    formTurma.reset();
    carregarTurmas();

  } catch (error) {
    console.error(error);
    alert("Erro ao criar turma.");
  }
});

turmasGrid.addEventListener("click", async (e) => {
  const btnEdit = e.target.closest(".btn-edit");
  if (!btnEdit) return;

  turmaEditandoId = btnEdit.dataset.turmaId;
  const cursoId = btnEdit.dataset.cursoId;

  editNomeTurma.value = btnEdit.dataset.nome;
  editStatus.value = btnEdit.dataset.status;

  await carregarCursosParaEdicao(cursoId);

  popupEditarTurma.classList.remove("hidden");
});


async function carregarCursosParaEdicao(cursoSelecionadoId) {
  try {
    editSelectCurso.innerHTML = `<option>Carregando...</option>`;

    const response = await fetch("http://localhost:5027/api/Curso");
    if (!response.ok) throw new Error();

    const cursos = await response.json();

    editSelectCurso.innerHTML = "";

    for (const curso of cursos) {
      const option = document.createElement("option");
      option.value = curso.id;
      option.textContent = curso.nome;

      if (curso.id == cursoSelecionadoId) {
        option.selected = true;
      }

      editSelectCurso.appendChild(option);
    }

  } catch (error) {
    console.error(error);
    editSelectCurso.innerHTML = `<option>Erro ao carregar</option>`;
  }
}

btnCancelarEdicaoTurma.addEventListener("click", () => {
  popupEditarTurma.classList.add("hidden");
  turmaEditandoId = null;
});

btnSalvarEdicaoTurma.addEventListener("click", async () => {
  if (!turmaEditandoId) return;

  const payload = {
    id: turmaEditandoId,
    nome: editNomeTurma.value.trim(),
    cursoId: Number(editSelectCurso.value),
    status: editStatus.value.toUpperCase()
  };

  try {
    const response = await fetch(
      `http://localhost:5027/api/Turma/atualizarTurma`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
      }
    );

    if (!response.ok) throw new Error();

    popupEditarTurma.classList.add("hidden");

    carregarTurmas(); 

  } catch (error) {
    console.error(error);
    alert("Erro ao atualizar turma");
  }
});

const professoresGrid = document.getElementById("professoresGrid");

function criarCardProfessor(nome, email, rm) {
  const card = document.createElement("div");
  card.classList.add("professor-card");

  card.innerHTML = `
    <h3><i class='bx bx-user'></i> ${nome}</h3>
    <p><i class='bx bx-envelope'></i> ${email ?? "Email não informado"}</p>
    <p><i class='bx bx-registered'></i> ${rm}</p>

  `;

  professoresGrid.appendChild(card);
}

async function carregarProfessores() {
  try {
    professoresGrid.innerHTML = "Carregando...";

    const response = await fetch(
      "http://localhost:5027/api/Professor/getAllProfessores"
    );

    if (!response.ok) throw new Error("Erro ao buscar professores");

    const professores = await response.json();

    professoresGrid.innerHTML = "";

    professores.forEach(prof => {
      criarCardProfessor(prof.nome, prof.email, prof.rm);
    });

  } catch (error) {
    console.error(error);
    professoresGrid.innerHTML = "<p>Erro ao carregar professores.</p>";
  }
}
