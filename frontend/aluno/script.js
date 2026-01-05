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

// ===== Alternar entre seções =====
const menuItems = document.querySelectorAll(".sidebar nav ul li");
const sections = document.querySelectorAll("main section");

menuItems.forEach(item => {
  item.addEventListener("click", () => {
    menuItems.forEach(i => i.classList.remove("active"));
    sections.forEach(sec => sec.classList.remove("active"));
    item.classList.add("active");
    document.getElementById(item.dataset.section).classList.add("active");
    if (item.dataset.section === "materias") {
      carregarMateriasDoAluno();
    }
    if (item.dataset.section === "relatorio") {
      carregarRelatorioAluno();
    }
  });
});

// ===== Alternar tema claro/escuro =====
const themeSwitch = document.getElementById("themeSwitch");
themeSwitch.addEventListener("change", () => {
  document.body.classList.toggle("light", themeSwitch.checked);
  updateChartColors(); // 🔥 Atualiza as cores dos gráficos ao mudar tema
});

const calendarDays = document.getElementById("calendarDays");
const monthYear = document.getElementById("monthYear");

const viewEventModal = document.getElementById("viewEventModal");
const viewTitle = document.getElementById("viewTitle");
const viewTime = document.getElementById("viewTime");
const viewDescription = document.getElementById("viewDescription");
const closeViewModal = document.getElementById("closeViewModal");

let currentDate = new Date();

// 🔹 Exemplo com múltiplos eventos no mesmo dia
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


document.addEventListener("DOMContentLoaded", () => {
  const mediaGeral = 9.4; // Mock
  const melhorCurso = "Desenvolvimento Web";
  const frequencia = 94;

  document.getElementById("mediaGeral").textContent = mediaGeral;
  document.getElementById("melhorCurso").textContent = melhorCurso;
  document.getElementById("frequencia").textContent = `${frequencia}%`;

  const nivelLabel = document.getElementById("nivelLabel");
  const nivelCard = document.getElementById("nivelCard");

  if (mediaGeral >= 8) {
    nivelLabel.textContent = "🏅 Ouro";
    nivelCard.style.background = "linear-gradient(135deg, #ffd70055, #ffcc00)";
  } else if (mediaGeral >= 6) {
    nivelLabel.textContent = "🥈 Prata";
    nivelCard.style.background = "linear-gradient(135deg, #c0c0c055, #aaaaaa)";
  } else {
    nivelLabel.textContent = "🥉 Bronze";
    nivelCard.style.background = "linear-gradient(135deg, #cd7f3255, #b87333)";
  }

  // === GRÁFICO DE EVOLUÇÃO ===
  const ctxEvolucao = document.getElementById("graficoEvolucao");
  window.graficoEvolucao = new Chart(ctxEvolucao, {
    type: "line",
    data: {
      labels: ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun"],
      datasets: [{
        label: "Notas",
        data: [6.5, 7.0, 7.8, 8.2, 8.5, 8.9],
        borderColor: "#ff0054",
        backgroundColor: "rgba(255, 0, 84, 0.2)",
        fill: true,
        tension: 0.3,
        pointRadius: 5,
        pointBackgroundColor: "#ff0054"
      }]
    },
    options: {
      plugins: {
        legend: {
          labels: { color: getTextColor() }
        }
      },
      scales: {
        x: { ticks: { color: getTextColor() } },
        y: { ticks: { color: getTextColor() }, beginAtZero: true, max: 10 }
      }
    }
  });

  // === GRÁFICO DE PIZZA ===
  const ctxPizza = document.getElementById("graficoPizza");
  window.graficoPizza = new Chart(ctxPizza, {
    type: "doughnut",
    data: {
      labels: ["Matemática", "Web", "Banco de Dados", "IA"],
      datasets: [{
        data: [4, 9.0, 8.0, 7],
        backgroundColor: ["#ff0054", "#00c2ff", "#ffaa00", "#9b5de5"],
        hoverOffset: 10
      }]
    },
    options: {
      plugins: {
        legend: {
          position: "bottom",
          labels: { color: getTextColor() }
        }
      }
    }
  });
});

// === Funções auxiliares ===
function getTextColor() {
  return document.body.classList.contains("light") ? "#222" : "#fff";
}

function updateChartColors() {
  const color = getTextColor();

  if (window.graficoEvolucao) {
    window.graficoEvolucao.options.plugins.legend.labels.color = color;
    window.graficoEvolucao.options.scales.x.ticks.color = color;
    window.graficoEvolucao.options.scales.y.ticks.color = color;
    window.graficoEvolucao.update();
  }

  if (window.graficoPizza) {
    window.graficoPizza.options.plugins.legend.labels.color = color;
    window.graficoPizza.update();
  }
}
const materiasView = document.getElementById("materiasView");
const materiaDetalhes = document.getElementById("materiaDetalhes");
const tituloMateria = document.getElementById("tituloMateria");
const listaArquivos = document.getElementById("listaArquivos");
const voltarMaterias = document.getElementById("voltarMaterias");

const API_BASE = "http://localhost:5027/api";

const materiasCards = document.getElementById("materiasCards");

function renderizarMaterias(materias) {
  if (!materiasCards) {
    console.error("Elemento materiasCards não encontrado");
    return;
  }

  materiasCards.innerHTML = "";

  if (!materias || materias.length === 0) {
    materiasCards.innerHTML = "<p>Nenhuma matéria encontrada.</p>";
    return;
  }

  materias.forEach(materia => {
    const card = document.createElement("div");
    card.classList.add("materia");

    card.innerHTML = `
      <i class="bx bx-book"></i>
      <h3>${materia.nome}</h3>
    `;

    card.addEventListener("click", () => abrirMateria(materia));
    materiasCards.appendChild(card);
  });
}

async function carregarMateriasDoAluno() {
  try {
    const usuarioLogado = JSON.parse(localStorage.getItem("usuarioLogado"));
    if (!usuarioLogado || !usuarioLogado.id) {
      console.warn("Usuário não logado ou id inexistente");
      return;
    }
    const alunoId = usuarioLogado.id;
    console.log("alunoId:", alunoId);

    const cursoResponse = await fetch(`${API_BASE}/Materia/getCursoId/${alunoId}`);
    const { cursoId } = await cursoResponse.json();

    const materiasResponse = await fetch(
      `${API_BASE}/Materia/getMateriasByCursoId/${cursoId}`
    );
    const materias = await materiasResponse.json();

    renderizarMaterias(materias);

  } catch (error) {
    console.error("Erro ao carregar matérias:", error);
  }
}


async function abrirMateria(materia) {
  try {
    tituloMateria.textContent = materia.nome;
    listaArquivos.innerHTML = "<li>Carregando conteúdos...</li>";

    const response = await fetch(
      `${API_BASE}/Conteudo/getAllConteudos/${materia.id}`
    );

    if (!response.ok) {
      throw new Error("Erro ao buscar conteúdos");
    }

    const data = await response.json();
    renderizarConteudos(data.conteudos);

    materiasView.style.display = "none";
    materiaDetalhes.classList.remove("hidden");

    voltarMaterias.addEventListener("click", () => {
      materiaDetalhes.classList.add("hidden");
      materiasView.style.display = "block";
    });

  } catch (error) {
    console.error(error);
    listaArquivos.innerHTML = `
      <li><i class='bx bx-error'></i> Erro ao carregar conteúdos</li>
    `;
  }
}

function renderizarConteudos(conteudos) {
  listaArquivos.innerHTML = "";

  if (!conteudos || conteudos.length === 0) {
    listaArquivos.innerHTML = `
      <li><i class='bx bx-info-circle'></i> Nenhum conteúdo disponível</li>
    `;
    return;
  }

  conteudos.forEach(conteudo => {
    const li = document.createElement("li");

    if (conteudo.tipo.toLowerCase() === "pdf") {
      li.innerHTML = `
        <i class='bx bx-file'></i>
        <a href="http://localhost:5027${conteudo.caminhoOuUrl}" target="_blank">
          ${conteudo.titulo}
        </a>
      `;
    } else {
      li.innerHTML = `
        <i class='bx bx-link'></i>
        <a href="${conteudo.caminhoOuUrl}" target="_blank">
          ${conteudo.titulo}
        </a>
      `;
    }

    listaArquivos.appendChild(li);
  });
}

async function buscarNomeMateria(materiaId) {
  try {
    const response = await fetch(
      `${API_BASE}/Materia/getNomeMateria/${materiaId}`
    );

    if (!response.ok) {
      throw new Error("Erro ao buscar nome da matéria");
    }

    const data = await response.json();
    return data.materiaNome; 
  } catch (error) {
    console.error(`Erro ao buscar nome da matéria ${materiaId}:`, error);
    return `Matéria ${materiaId}`;
  }
}


const relatorioContainer = document.getElementById("relatorioContainer");

async function carregarRelatorioAluno() {
  try {
    const usuarioLogado = JSON.parse(localStorage.getItem("usuarioLogado"));

    if (!usuarioLogado || !usuarioLogado.id) {
      relatorioContainer.innerHTML = "<p>Usuário não identificado.</p>";
      return;
    }

    const alunoId = usuarioLogado.id;

    const response = await fetch(`${API_BASE}/Nota/aluno/${alunoId}`);
    if (!response.ok) throw new Error("Erro ao buscar notas");

    const notas = await response.json();

    if (!notas || notas.length === 0) {
      relatorioContainer.innerHTML = "<p>Nenhuma nota encontrada.</p>";
      return;
    }

    relatorioContainer.innerHTML = "";

    for (const nota of notas) {
      await renderizarCardNota(nota);
    }

  } catch (error) {
    console.error("Erro ao carregar relatório:", error);
    relatorioContainer.innerHTML = "<p>Erro ao carregar relatório.</p>";
  }
}

async function renderizarCardNota(nota) {
  const nomeMateria = await buscarNomeMateria(nota.materiaId);

  const media = ((nota.nota1 + nota.nota2) / 2).toFixed(1);

  let statusHtml = "";
  if (nota.status === "APROVADO") {
    statusHtml = `<span class="status-aprovado">Aprovado</span>`;
  } else if (nota.status === "RECUPERACAO") {
    statusHtml = `<span class="status-recuperacao">Recuperação</span>`;
  } else {
    statusHtml = `<span class="status-reprovado">Reprovado</span>`;
  }

  const card = document.createElement("div");
  card.classList.add("materia-card");

  card.innerHTML = `
    <div class="materia-header">
      <h3>${nomeMateria}</h3>
      <div class="media-geral">
        Média: ${media} | ${statusHtml}
      </div>
    </div>

    <table class="tabela-notas">
      <thead>
        <tr>
          <th>Avaliação</th>
          <th>Nota</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>Nota 1</td>
          <td>${nota.nota1.toFixed(1)}</td>
        </tr>
        <tr>
          <td>Nota 2</td>
          <td>${nota.nota2.toFixed(1)}</td>
        </tr>
      </tbody>
    </table>

    <p class="faltas">
      Faltas: ${nota.qtdFaltas}
    </p>
  `;

  relatorioContainer.appendChild(card);
}




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

    // Preencher HTML
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
