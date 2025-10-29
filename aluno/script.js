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
  });
});

// ===== Alternar tema claro/escuro =====
const themeSwitch = document.getElementById("themeSwitch");
themeSwitch.addEventListener("change", () => {
  document.body.classList.toggle("light", themeSwitch.checked);
  updateChartColors(); // 🔥 Atualiza as cores dos gráficos ao mudar tema
});

// ===== CALENDÁRIO =====
const calendarDays = document.getElementById("calendarDays");
const monthYear = document.getElementById("monthYear");
const eventModal = document.getElementById("eventModal");
const eventTitle = document.getElementById("eventTitle");
const eventTime = document.getElementById("eventTime");
const saveEvent = document.getElementById("saveEvent");
const closeModal = document.getElementById("closeModal");

let currentDate = new Date();
let selectedDate = null;
let events = {}; 

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
    const dateKey = `${year}-${month + 1}-${day}`;
    const dayDiv = document.createElement("div");
    dayDiv.classList.add("calendar-day");
    dayDiv.textContent = day;

    if (events[dateKey]) {
      const dot = document.createElement("div");
      dot.classList.add("event-dot");
      dayDiv.appendChild(dot);
    }

    dayDiv.addEventListener("click", () => {
      selectedDate = dateKey;
      eventTitle.value = "";
      eventTime.value = "";
      eventModal.style.display = "flex";
    });

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

saveEvent.onclick = () => {
  if (eventTitle.value.trim() === "") return alert("Informe o título do evento!");

  if (!events[selectedDate]) events[selectedDate] = [];
  events[selectedDate].push({
    title: eventTitle.value,
    time: eventTime.value,
  });

  eventModal.style.display = "none";
  renderCalendar();
};

closeModal.onclick = () => (eventModal.style.display = "none");
window.onclick = (e) => {
  if (e.target === eventModal) eventModal.style.display = "none";
};

renderCalendar();

document.addEventListener("DOMContentLoaded", () => {
  const mediaGeral = 3.4; // Mock
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

// Dados simulados das matérias e arquivos
const materiasData = {
  matematica: [
    "Lista de Exercícios 1.pdf",
    "Funções e Gráficos.pptx",
    "Aula 5 - Equações.mp4"
  ],
  programacao: [
    "Projeto HTML - Estrutura Base.zip",
    "Guia CSS Flexbox.pdf",
    "Vídeo Aula - DOM e Eventos.mp4"
  ],
  banco: [
    "Modelo Relacional.pdf",
    "Aula 3 - SQL Básico.mp4",
    "Script de Criação de Tabelas.sql"
  ],
  ia: [
    "Introdução à IA.pdf",
    "Redes Neurais - Material de Apoio.pdf"
  ]
};

const materiasView = document.getElementById("materiasView");
const materiaDetalhes = document.getElementById("materiaDetalhes");
const tituloMateria = document.getElementById("tituloMateria");
const listaArquivos = document.getElementById("listaArquivos");
const voltarMaterias = document.getElementById("voltarMaterias");

// Evento: clicar em uma matéria
document.querySelectorAll(".materia").forEach(card => {
  card.addEventListener("click", () => {
    const materiaKey = card.dataset.materia;
    const arquivos = materiasData[materiaKey];
    tituloMateria.textContent = card.querySelector("h3").textContent;

    listaArquivos.innerHTML = "";
    arquivos.forEach(nome => {
      const li = document.createElement("li");
      li.innerHTML = `<i class='bx bx-link'></i> ${nome}`;
      listaArquivos.appendChild(li);
    });

    materiasView.style.display = "none";
    materiaDetalhes.classList.remove("hidden");
  });
});

// Evento: voltar para a lista de matérias
voltarMaterias.addEventListener("click", () => {
  materiaDetalhes.classList.add("hidden");
  materiasView.style.display = "block";
});


// Dados simulados — pode integrar com API futuramente
const notasAluno = {
  "Matemática": [
    { avaliacao: "Prova 1", nota: 7.5 },
    { avaliacao: "Trabalho 1", nota: 8.2 },
    { avaliacao: "Prova 2", nota: 9.0 },
    { avaliacao: "Trabalho Final", nota: 8.8 }
  ],
  "Programação Web": [
    { avaliacao: "Atividade 1", nota: 9.5 },
    { avaliacao: "Projeto 1", nota: 8.9 },
    { avaliacao: "Prova Final", nota: 9.7 }
  ],
  "Banco de Dados": [
    { avaliacao: "Prova 1", nota: 6.5 },
    { avaliacao: "Atividade 2", nota: 7.0 },
    { avaliacao: "Trabalho Final", nota: 7.8 }
  ],
  "Inteligência Artificial": [
    { avaliacao: "Trabalho 1", nota: 6.0 },
    { avaliacao: "Prova 1", nota: 5.8 },
    { avaliacao: "Projeto Final", nota: 7.2 }
  ]
};

// === GERAR CONTEÚDO NA TELA ===
const container = document.getElementById("relatorioContainer");

Object.entries(notasAluno).forEach(([materia, avaliacoes]) => {
  const media = (avaliacoes.reduce((acc, a) => acc + a.nota, 0) / avaliacoes.length).toFixed(1);
  
  let status = "";
  if (media >= 7) status = `<span class="status-aprovado">Aprovado</span>`;
  else if (media >= 5) status = `<span class="status-recuperacao">Recuperação</span>`;
  else status = `<span class="status-reprovado">Reprovado</span>`;

  const card = document.createElement("div");
  card.classList.add("materia-card");

  card.innerHTML = `
    <div class="materia-header">
      <h3>${materia}</h3>
      <div class="media-geral">Média: ${media} | ${status}</div>
    </div>
    <table class="tabela-notas">
      <thead>
        <tr>
          <th>Avaliação</th>
          <th>Nota</th>
        </tr>
      </thead>
      <tbody>
        ${avaliacoes.map(a => `
          <tr>
            <td>${a.avaliacao}</td>
            <td>${a.nota.toFixed(1)}</td>
          </tr>
        `).join("")}
      </tbody>
    </table>
  `;

  container.appendChild(card);
});

// === GERAR PDF COM CORES FIXAS (PRETO E VERMELHO) ===
document.getElementById("gerarRelatorioGeral").addEventListener("click", () => {
  const relatorioClone = container.cloneNode(true);
  relatorioClone.style.background = "#ffffff";
  relatorioClone.style.color = "#000000";
  relatorioClone.style.padding = "20px";
  relatorioClone.style.fontFamily = "Arial, sans-serif";
  relatorioClone.style.border = "2px solid #ff0000";
  relatorioClone.style.borderRadius = "8px";

  // Ajusta elementos internos
  relatorioClone.querySelectorAll("*").forEach(el => {
    el.style.color = "#000000";
    el.style.backgroundColor = "#ffffff";
    el.style.borderColor = "#ff0000";
  });

  // Título vermelho e destacado
  const titulo = document.createElement("h2");
  titulo.textContent = "📘 Relatório Geral de Desempenho";
  titulo.style.textAlign = "center";
  titulo.style.marginBottom = "20px";
  titulo.style.color = "#ff0000";
  titulo.style.textTransform = "uppercase";
  titulo.style.letterSpacing = "1px";
  relatorioClone.prepend(titulo);

  // Estilização específica para tabelas e status
  relatorioClone.querySelectorAll("table").forEach(tabela => {
    tabela.style.width = "100%";
    tabela.style.borderCollapse = "collapse";
    tabela.style.marginTop = "10px";
  });

  relatorioClone.querySelectorAll("th, td").forEach(cel => {
    cel.style.border = "1px solid #ff0000";
    cel.style.padding = "6px";
    cel.style.textAlign = "center";
  });

  relatorioClone.querySelectorAll(".materia-header h3").forEach(h3 => {
    h3.style.color = "#ff0000";
  });

  relatorioClone.querySelectorAll(".media-geral").forEach(media => {
    media.style.color = "#000000";
    media.style.fontWeight = "bold";
  });

  // Configuração do PDF
  const opt = {
    margin: 20,
    filename: "Relatorio_Geral_Aluno.pdf",
    image: { type: "jpeg", quality: 1 },
    html2canvas: { scale: 2, backgroundColor: "#ffffff" },
    jsPDF: { unit: "mm", format: "a4", orientation: "portrait" }
  };

  html2pdf().set(opt).from(relatorioClone).save();
});
