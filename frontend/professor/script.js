// ===== Alternar entre seções =====
const menuItems = document.querySelectorAll(".sidebar nav ul li");
const sections = document.querySelectorAll("main section");

menuItems.forEach(item => {
  item.addEventListener("click", () => {
    menuItems.forEach(i => i.classList.remove("active"));
    sections.forEach(sec => sec.classList.remove("active"));
    item.classList.add("active");
    document.getElementById(item.dataset.section).classList.add("active");
  });
});

// ===== Alternar tema claro/escuro =====
const themeSwitch = document.getElementById("themeSwitch");
themeSwitch.addEventListener("change", () => {
  document.body.classList.toggle("light", themeSwitch.checked);
});




//toggle ocult sidebar
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


//ultimo evento 

async function carregarProximoEvento() {
  try {
    const response = await fetch('http://localhost:5027/api/Evento/proximoEvento');

    if (!response.ok) {
      throw new Error('Erro ao buscar próximo evento');
    }

    const evento = await response.json();

    const data = new Date(evento.horario);

    const dia = data.getDate();
    const mes = data.toLocaleString('pt-BR', { month: 'short' }).toUpperCase();
    const horario = data.toLocaleTimeString('pt-BR', {
      hour: '2-digit',
      minute: '2-digit'
    });

    document.getElementById('evento-dia').textContent = dia;
    document.getElementById('evento-mes').textContent = mes;
    document.getElementById('evento-titulo').textContent = evento.titulo;
    document.getElementById('evento-horario').textContent = horario;
    document.getElementById('evento-descricao').textContent = evento.descricao;


  } catch (error) {
    console.error(error);
    document.getElementById('evento-titulo').textContent = 'Nenhum evento encontrado';
  }
}

carregarProximoEvento();

const API_BASE = "http://localhost:5027/api";

async function carregarResumoProfessor() {
  try {
    const usuarioLogado = JSON.parse(localStorage.getItem("usuarioLogado"));

    if (!usuarioLogado || !usuarioLogado.id) {
      console.warn("Professor não identificado");
      return;
    }

    const professorId = usuarioLogado.id;

    await Promise.all([
      carregarAlunosAtivos(professorId),
      carregarQtdTurmas(professorId),
      carregarEventosProximaSemana()
    ]);

  } catch (error) {
    console.error("Erro ao carregar resumo:", error);
  }
}


async function carregarAlunosAtivos(professorId) {
  const response = await fetch(
    `${API_BASE}/Matricula/getAlunosAtivosProfessor/${professorId}`
  );

  if (!response.ok) throw new Error("Erro ao buscar alunos ativos");

  const qtdAlunos = await response.json();

  document.getElementById("qtdAlunosAtivos").textContent = qtdAlunos;
}

async function carregarQtdTurmas(professorId) {
  const response = await fetch(
    `${API_BASE}/Curso/getQntdCursosProf/${professorId}`
  );

  if (!response.ok) throw new Error("Erro ao buscar turmas");

  const qtdTurmas = await response.json();

  document.getElementById("qtdTurmas").textContent = qtdTurmas;
}

async function carregarEventosProximaSemana() {
  const response = await fetch(
    `${API_BASE}/Evento/getNextWeekEvents`
  );

  if (!response.ok) throw new Error("Erro ao buscar eventos");

  const eventos = await response.json();

  document.getElementById("qtdEventosSemana").textContent = eventos;
}

document.addEventListener("DOMContentLoaded", carregarResumoProfessor);


const calendarDays = document.getElementById("calendarDays");
const monthYear = document.getElementById("monthYear");

const viewEventModal = document.getElementById("viewEventModal");
const viewTitle = document.getElementById("viewTitle");
const viewTime = document.getElementById("viewTime");
const viewDescription = document.getElementById("viewDescription");
const closeViewModal = document.getElementById("closeViewModal");

let currentDate = new Date();

let events = {};

async function carregarEventos() {
  try {
    const response = await fetch("http://localhost:5027/api/Evento/getAllEvents");
    const data = await response.json();

    events = {};

    data.forEach(ev => {
      const date = new Date(ev.horario);

      // Normaliza para o início do dia (00:00)
      date.setHours(0, 0, 0, 0);

      const dayTimestamp = date.getTime();

      if (!events[dayTimestamp]) {
        events[dayTimestamp] = [];
      }

      events[dayTimestamp].push({
        title: ev.titulo,
        time: new Date(ev.horario).toLocaleTimeString("pt-BR", {
          hour: "2-digit",
          minute: "2-digit"
        }),
        description: ev.descricao
      });
    });

  } catch (error) {
    console.error("Erro ao carregar eventos:", error);
  }
}


function renderCalendar() {
  const year = currentDate.getFullYear();
  const month = currentDate.getMonth();

  monthYear.textContent = `${currentDate.toLocaleString("pt-BR", {
    month: "long",
  })} ${year}`;

  calendarDays.innerHTML = "";

  const firstDay = new Date(year, month, 1).getDay();
  const lastDate = new Date(year, month + 1, 0).getDate();

  for (let i = 0; i < firstDay; i++) {
    const empty = document.createElement("div");
    calendarDays.appendChild(empty);
  }

  for (let day = 1; day <= lastDate; day++) {
    const dateObj = new Date(year, month, day);
    dateObj.setHours(0, 0, 0, 0);
    const dateKey = dateObj.getTime();    const dayDiv = document.createElement("div");
    dayDiv.classList.add("calendar-day");
    dayDiv.textContent = day;

    if (events[dateKey]) {
      const dot = document.createElement("div");
      dot.classList.add("event-dot");
      dayDiv.appendChild(dot);

      dayDiv.addEventListener("click", () => {
        const eventList = events[dateKey];

        // Limpa o conteúdo anterior
        viewDescription.innerHTML = "";

        // Adiciona cada evento como um bloco
        eventList.forEach(ev => {
          const eventBlock = document.createElement("div");
          eventBlock.classList.add("event-item");
          eventBlock.innerHTML = `
            <p><strong>${ev.title}</strong></p>
            <p><i class='bx bx-time-five'></i> ${ev.time || "—"}</p>
            <p>${ev.description || "Sem descrição."}</p>
          `;
          viewDescription.appendChild(eventBlock);
        });

        // Mostra o modal
        viewEventModal.style.display = "flex";
      });
    }

    calendarDays.appendChild(dayDiv);
  }
}

document.getElementById("prevMonth").onclick = () => {
  currentDate.setMonth(currentDate.getMonth() - 1);
  renderCalendar();
};
document.getElementById("nextMonth").onclick = () => {
  currentDate.setMonth(currentDate.getMonth() + 1);
  renderCalendar();
};
document.getElementById("prevYear").onclick = () => {
  currentDate.setFullYear(currentDate.getFullYear() - 1);
  renderCalendar();
};
document.getElementById("nextYear").onclick = () => {
  currentDate.setFullYear(currentDate.getFullYear() + 1);
  renderCalendar();
};

closeViewModal.onclick = () => (viewEventModal.style.display = "none");
window.onclick = (e) => {
  if (e.target === viewEventModal) viewEventModal.style.display = "none";
};

(async () => {
  await carregarEventos();
  renderCalendar();
})();

document.getElementById("btnBuscarAluno").addEventListener("click", buscarBoletimAluno);

async function buscarBoletimAluno() {
  const valorBusca = document.getElementById("searchAluno").value.trim();

  if (!valorBusca) {
    return;
  }

  let url = "";

  // Verifica se é matrícula (somente números)
  if (/^\d+$/.test(valorBusca)) {
    url = `${API_BASE}/Nota/aluno/${valorBusca}/getAllNotasByMatriculaId`;
  } else {
    url = `${API_BASE}/Nota/aluno/getAllNotasByNome?nome=${encodeURIComponent(valorBusca)}`;
  }

  try {
    const response = await fetch(url);
    if (!response.ok) throw new Error("Aluno não encontrado");

    const notas = await response.json();

    if (!notas || notas.length === 0) {
      return;
    }

    renderizarBoletim(notas);

  } catch (error) {
    console.error(error);
  }
}
async function renderizarBoletim(notas) {
  const resultadoBox = document.getElementById("resultadoAluno");
  resultadoBox.classList.remove("hidden");

  // Limpa resultados anteriores
  resultadoBox.innerHTML = "<h3>Resultados da busca</h3>";

  const notasPorAluno = agruparNotasPorAluno(notas);

  for (const alunoId in notasPorAluno) {
    const notasAluno = notasPorAluno[alunoId];

    const alunoInfo = await buscarInfoAluno(alunoId);

    const alunoDiv = document.createElement("div");
    alunoDiv.classList.add("boletim-aluno");

    alunoDiv.innerHTML = `
      <h4>Informações do aluno</h4>
      <p><strong>Nome:</strong> ${alunoInfo.nome}</p>
      <p><strong>Curso:</strong> ${alunoInfo.cursoNome}</p>
      <p><strong>Matricula:</strong> ${alunoInfo.matriculaId}</p>


      <table class="tabela-notas">
        <thead>
          <tr>
            <th>Disciplina</th>
            <th>Média Final</th>
            <th>Frequência</th>
          </tr>
        </thead>
        <tbody></tbody>
      </table>

      <button class="btn-pdf">
        <i class='bx bx-file'></i> Gerar Boletim em PDF
      </button>

      <hr/>
    `;

    const tbody = alunoDiv.querySelector("tbody");

    for (const nota of notasAluno) {
      const nomeMateria = await buscarNomeMateria(nota.materiaId);

      const frequencia = Math.max(
        0,
        Math.round(((70 - nota.qtdFaltas) / 70) * 100)
      );

      const tr = document.createElement("tr");
      tr.innerHTML = `
        <td>${nomeMateria}</td>
        <td>${nota.media.toFixed(1)}</td>
        <td>${frequencia}%</td>
      `;

      tbody.appendChild(tr);
    }

    alunoDiv.querySelector(".btn-pdf").addEventListener("click", () => {
      gerarPdfBoletim(alunoInfo, notasAluno);
    });

    resultadoBox.appendChild(alunoDiv);
  }
}


async function buscarNomeMateria(materiaId) {
  try {
    const response = await fetch(
      `${API_BASE}/Materia/getNomeMateria/${materiaId}`
    );

    if (!response.ok) throw new Error();

    const data = await response.json();
    return data.materiaNome; 

  } catch {
    return `Matéria ${materiaId}`;
  }
}

function agruparNotasPorAluno(notas) {
  return notas.reduce((acc, nota) => {
    if (!acc[nota.alunoId]) {
      acc[nota.alunoId] = [];
    }
    acc[nota.alunoId].push(nota);
    return acc;
  }, {});
}

async function buscarInfoAluno(alunoId) {
  const response = await fetch(
    `http://localhost:5027/api/Aluno/contextMe/${alunoId}`
  );

  if (!response.ok) {
    return { nome: `Aluno ${alunoId}`, curso: "—" };
  }

  return await response.json(); 
}
const selectCurso = document.getElementById("selectCurso");
const alunosContainer = document.getElementById("alunosContainer");
const formNota = document.getElementById("formNota");
const materiaSelect = document.getElementById("materiaSelect");

let alunoSelecionadoId = null;
let cursoSelecionadoId = null;

/* =============================
   CARREGAR CURSOS DO PROFESSOR
============================= */
async function carregarCursos() {
  const usuarioLogado = JSON.parse(localStorage.getItem("usuarioLogado"));

  if (!usuarioLogado?.id) {
    console.warn("Professor não identificado");
    return;
  }

  const response = await fetch(
    `http://localhost:5027/api/Curso/getAllCursosProf/${usuarioLogado.id}`
  );

  const cursos = await response.json();

  cursos.forEach(curso => {
    const option = document.createElement("option");
    option.value = curso.id;
    option.textContent = curso.nome;
    selectCurso.appendChild(option);
  });
}

carregarCursos();

/* =============================
   AO SELECIONAR CURSO
============================= */
selectCurso.addEventListener("change", async () => {
  cursoSelecionadoId = selectCurso.value;
  alunosContainer.innerHTML = "";
  formNota.classList.add("hidden");

  if (!cursoSelecionadoId) return;

  const response = await fetch(
    `http://localhost:5027/api/Aluno/getAllAlunosByCurso/${cursoSelecionadoId}`
  );

  const alunos = await response.json();
  alunosContainer.classList.remove("hidden");

  alunos.forEach(aluno => {
    const card = document.createElement("div");
    card.classList.add("aluno-card");

    card.innerHTML = `
      <div>
        <h4>${aluno.nome}</h4>
        <p>Email: ${aluno.email}</p>
        <p>CPF: ${aluno.cpf}</p>
        <p>Matrícula: ${aluno.matriculaId}</p>
      </div>
      <button class="btn-lancar">Lançar Nota</button>
    `;

    card.querySelector("button").addEventListener("click", () => {
      // Remove seleção dos outros alunos
      document
        .querySelectorAll(".aluno-card")
        .forEach(c => c.classList.remove("selecionado"));

      // Marca este como selecionado
      card.classList.add("selecionado");

      abrirFormularioNota(aluno.matriculaId);
    });

    alunosContainer.appendChild(card);
  });
});

/* =============================
   ABRIR FORMULÁRIO DE NOTA
============================= */
async function abrirFormularioNota(alunoId) {
  alunoSelecionadoId = alunoId;
  formNota.classList.remove("hidden");
  await carregarMateriasPorCurso(cursoSelecionadoId);
}

/* =============================
   CARREGAR MATÉRIAS DO CURSO
============================= */
async function carregarMateriasPorCurso(cursoId) {
  const response = await fetch(
    `http://localhost:5027/api/Materia/getMateriasByCursoId/${cursoId}`
  );

  const materias = await response.json();
  materiaSelect.innerHTML = `<option value="">Selecione a matéria</option>`;

  materias.forEach(materia => {
    const option = document.createElement("option");
    option.value = materia.id;
    option.textContent = materia.nome;
    materiaSelect.appendChild(option);
  });
}

/* =============================
   SALVAR NOTA
============================= */
document.getElementById("btnSalvarNota").addEventListener("click", async () => {
  const payload = {
    alunoId: alunoSelecionadoId,
    materiaId: Number(materiaSelect.value),
    nota1: Number(document.getElementById("nota1").value),
    nota2: Number(document.getElementById("nota2").value),
    qtdFaltas: Number(document.getElementById("qtdFaltas").value)
  };

  if (!payload.materiaId) {
    alert("Selecione uma matéria");
    return;
  }

  try {
    const response = await fetch(
      "http://localhost:5027/api/Nota/adicionarNota",
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
      }
    );

    if (!response.ok) throw new Error();

    alert("Nota lançada com sucesso!");
    formNota.classList.add("hidden");
  } catch {
    alert("Erro ao lançar nota");
  }
});


const selectCursoChamada = document.getElementById("selectCursoChamada");
const selectTurmaChamada = document.getElementById("selectTurmaChamada");
const selectMateriaChamada = document.getElementById("selectMateriaChamada");

const turmaChamadaContainer = document.getElementById("turmaChamadaContainer");
const materiaChamadaContainer = document.getElementById("materiaChamadaContainer");
const alunosChamadaContainer = document.getElementById("alunosChamadaContainer");

const tabelaChamada = document.getElementById("tabelaChamada");


async function carregarCursosChamada() {
  const usuarioLogado = JSON.parse(localStorage.getItem("usuarioLogado"));
  if (!usuarioLogado?.id) return;

  const response = await fetch(
    `http://localhost:5027/api/Curso/getAllCursosProf/${usuarioLogado.id}`
  );
  const cursos = await response.json();

  cursos.forEach(curso => {
    const option = document.createElement("option");
    option.value = curso.id;
    option.textContent = curso.nome;
    selectCursoChamada.appendChild(option);
  });
}

carregarCursosChamada();

selectCursoChamada.addEventListener("change", async () => {
  const cursoId = selectCursoChamada.value;

  selectTurmaChamada.innerHTML =
    `<option value="">-- Selecione a turma --</option>`;
  selectMateriaChamada.innerHTML =
    `<option value="">-- Selecione a matéria --</option>`;

  tabelaChamada.innerHTML = "";

  turmaChamadaContainer.classList.add("hidden");
  materiaChamadaContainer.classList.add("hidden");
  alunosChamadaContainer.classList.add("hidden");

  if (!cursoId) return;

  const response = await fetch(
    `http://localhost:5027/api/Turma/getTurmasByCursoId/${cursoId}`
  );

  const turmas = await response.json();

  turmas.forEach(turma => {
    const option = document.createElement("option");
    option.value = turma.id;
    option.textContent = turma.nome;
    selectTurmaChamada.appendChild(option);
  });

  turmaChamadaContainer.classList.remove("hidden");
});

selectTurmaChamada.addEventListener("change", async () => {
  const cursoId = selectCursoChamada.value;

  selectMateriaChamada.innerHTML =
    `<option value="">-- Selecione a matéria --</option>`;
  tabelaChamada.innerHTML = "";
  alunosChamadaContainer.classList.add("hidden");

  if (!cursoId) return;

  const response = await fetch(
    `http://localhost:5027/api/Materia/getMateriasByCursoId/${cursoId}`
  );

  const materias = await response.json();

  materias.forEach(materia => {
    const option = document.createElement("option");
    option.value = materia.id;
    option.textContent = materia.nome;
    selectMateriaChamada.appendChild(option);
  });

  materiaChamadaContainer.classList.remove("hidden");
});

selectMateriaChamada.addEventListener("change", async () => {
  const turmaId = selectTurmaChamada.value;
  const materiaId = selectMateriaChamada.value;

  tabelaChamada.innerHTML = "";
  alunosChamadaContainer.classList.add("hidden");

  if (!turmaId || !materiaId) return;

  const response = await fetch(
    `http://localhost:5027/api/Aluno/getAllAlunosByTurmaId/${turmaId}`
  );

  const alunos = await response.json();

  alunos.forEach(aluno => {
    const tr = document.createElement("tr");

    tr.innerHTML = `
      <td>${aluno.nome}</td>
      <td>${aluno.matriculaId}</td>
      <td style="text-align:center">
        <input 
          type="checkbox"
          class="checkbox-falta"
          data-matricula-id="${aluno.matriculaId}"
        />
      </td>
    `;

    tabelaChamada.appendChild(tr);
  });

  alunosChamadaContainer.classList.remove("hidden");
});

document.getElementById("btnSalvarChamada").addEventListener("click", async () => {
  const materiaId = Number(selectMateriaChamada.value);
  const turmaId = Number(selectTurmaChamada.value);

  if (!materiaId) return;

  const chamadaPayload = [];

  document.querySelectorAll(".checkbox-falta").forEach(cb => {
    chamadaPayload.push({
      matriculaId: Number(cb.dataset.matriculaId),
      materiaId,
      faltou: cb.checked,
      turmaId
    });
  });

  if (!chamadaPayload.length) return;

  try {
  console.log(chamadaPayload);

  const response = await fetch(
    "http://localhost:5027/api/Chamada/realizarChamada",
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(chamadaPayload)
    }
  );

  if (response.status === 204) {
    mostrarSucessoChamada();
    return;
  } 
  else if (response.status === 409) {
    const erro = await response.json();

    if (erro.tipo === "CHAMADA_JA_REALIZADA") {
      mostrarPopupChamadaJaRealizada(erro.mensagem);
      return;
    }
  } 
  else {
    throw new Error("Erro ao salvar chamada");
  }

} catch (error) {
  console.error(error);
  alert("Erro ao salvar chamada");
}});

function mostrarSucessoChamada() {
  document.getElementById("chamadaSucesso").classList.remove("hidden");

  // bloqueia edição
  document.querySelectorAll(".checkbox-falta").forEach(cb => {
    cb.disabled = true;
  });

  document.getElementById("btnSalvarChamada").disabled = true;
}


document.getElementById("btnNovaChamada").addEventListener("click", () => {
  document.getElementById("chamadaSucesso").classList.add("hidden");

  selectMateriaChamada.value = "";
  tabelaChamada.innerHTML = "";
  alunosChamadaContainer.classList.add("hidden");

  document.getElementById("btnSalvarChamada").disabled = false;
});

function mostrarPopupChamadaJaRealizada(mensagem) {
  const banner = document.getElementById("chamadaBanner");
  const mensagemEl = document.getElementById("chamadaBannerMensagem");
  const btnCancelar = document.getElementById("btnCancelarSubstituicao");
  const btnConfirmar = document.getElementById("btnConfirmarSubstituicao");

  mensagemEl.textContent = mensagem;

  banner.classList.remove("hidden");

  btnCancelar.onclick = () => {
    banner.classList.add("hidden");
  };

  btnConfirmar.onclick = async () => {
    banner.classList.add("hidden");
    await substituirChamada();
  };
}


async function substituirChamada() {
  try {
    const materiaId = Number(selectMateriaChamada.value);
    const turmaId = Number(selectTurmaChamada.value);

    const payload = [];

    document.querySelectorAll(".checkbox-falta").forEach(cb => {
      payload.push({
        matriculaId: Number(cb.dataset.matriculaId),
        turmaId,
        materiaId,
        faltou: cb.checked,
        horarioChamada: new Date().toISOString()
      });
    });

    const response = await fetch(
      "http://localhost:5027/api/Chamada/atualizarChamada",
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
      }
    );

    if (!response.ok) {
      throw new Error("Erro ao substituir chamada");
    }

    mostrarSucessoChamada();

  } catch (error) {
    console.error(error);
    alert("Erro ao substituir a chamada");
  }
}


const selectCursoConteudo = document.getElementById("selectCursoConteudo");
const selectTurmaConteudo = document.getElementById("selectTurmaConteudo");
const selectMateriaConteudo = document.getElementById("selectMateriaConteudo");
const tipoConteudo = document.getElementById("tipoConteudo");

const turmaConteudoContainer = document.getElementById("turmaConteudoContainer");
const materiaConteudoContainer = document.getElementById("materiaConteudoContainer");
const tipoConteudoContainer = document.getElementById("tipoConteudoContainer");
const formConteudo = document.getElementById("formConteudo");

const campoLink = document.getElementById("campoLink");
const campoPdf = document.getElementById("campoPdf");

async function carregarCursosConteudo() {
  const usuarioLogado = JSON.parse(localStorage.getItem("usuarioLogado"));
  if (!usuarioLogado?.id) return;

  const response = await fetch(
    `http://localhost:5027/api/Curso/getAllCursosProf/${usuarioLogado.id}`
  );

  const cursos = await response.json();

  cursos.forEach(curso => {
    const option = document.createElement("option");
    option.value = curso.id;
    option.textContent = curso.nome;
    selectCursoConteudo.appendChild(option);
  });
}

carregarCursosConteudo();

selectCursoConteudo.addEventListener("change", async () => {
  const cursoId = selectCursoConteudo.value;

  selectTurmaConteudo.innerHTML = `<option value="">-- Selecione a turma --</option>`;
  turmaConteudoContainer.classList.add("hidden");
  materiaConteudoContainer.classList.add("hidden");
  tipoConteudoContainer.classList.add("hidden");
  formConteudo.classList.add("hidden");

  if (!cursoId) return;

  const response = await fetch(
    `http://localhost:5027/api/Turma/getTurmasByCursoId/${cursoId}`
  );

  const turmas = await response.json();

  turmas.forEach(turma => {
    const option = document.createElement("option");
    option.value = turma.id;
    option.textContent = turma.nome;
    selectTurmaConteudo.appendChild(option);
  });

  turmaConteudoContainer.classList.remove("hidden");
});


selectTurmaConteudo.addEventListener("change", async () => {
  const cursoId = selectCursoConteudo.value;
  const turmaId = selectTurmaConteudo.value;

  selectMateriaConteudo.innerHTML = `<option value="">-- Selecione a matéria --</option>`;
  materiaConteudoContainer.classList.add("hidden");
  tipoConteudoContainer.classList.add("hidden");
  formConteudo.classList.add("hidden");

  if (!turmaId) return;

  const response = await fetch(
    `http://localhost:5027/api/Materia/getMateriasByCursoId/${cursoId}`
  );

  const materias = await response.json();

  materias.forEach(materia => {
    const option = document.createElement("option");
    option.value = materia.id;
    option.textContent = materia.nome;
    selectMateriaConteudo.appendChild(option);
  });

  materiaConteudoContainer.classList.remove("hidden");
});


selectMateriaConteudo.addEventListener("change", () => {
  if (!selectMateriaConteudo.value) return;
  tipoConteudoContainer.classList.remove("hidden");
});

tipoConteudo.addEventListener("change", () => {
  campoLink.classList.add("hidden");
  campoPdf.classList.add("hidden");
  formConteudo.classList.add("hidden");

  if (tipoConteudo.value === "link") {
    campoLink.classList.remove("hidden");
  }

  if (tipoConteudo.value === "pdf") {
    campoPdf.classList.remove("hidden");
  }

  formConteudo.classList.remove("hidden");
});

document.getElementById("btnPublicarConteudo").addEventListener("click", async () => {
  const titulo = document.getElementById("tituloConteudo").value;
  const materiaId = Number(selectMateriaConteudo.value);
  const turmaId = Number(selectTurmaConteudo.value);

  if (!titulo || !materiaId || !turmaId) {
    alert("Preencha todos os campos obrigatórios");
    return;
  }

  try {
    // 🔗 LINK
    if (tipoConteudo.value === "link") {
      const url = document.getElementById("urlConteudo").value;

      const payload = {
        titulo,
        materiaId,
        url,
        turmaId
      };

      await fetch("http://localhost:5027/api/Conteudo/upload/link", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
      });
    }

    // 📄 PDF
    if (tipoConteudo.value === "pdf") {
      const arquivo = document.getElementById("arquivoPdf").files[0];
      if (!arquivo) {
        alert("Selecione um PDF");
        return;
      }

      const formData = new FormData();
      formData.append("Titulo", titulo);
      formData.append("MateriaId", materiaId);
      formData.append("TurmaId", turmaId);
      formData.append("Arquivo", arquivo);

      await fetch("http://localhost:5027/api/Conteudo/upload/pdf", {
        method: "POST",
        body: formData
      });
    }

    alert("Conteúdo publicado com sucesso!");
    formConteudo.classList.add("hidden");

  } catch (error) {
    console.error(error);
    alert("Erro ao publicar conteúdo");
  }
});
