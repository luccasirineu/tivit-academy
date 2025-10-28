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
  const mediaGeral = 8.4; // Mock
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
