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

// ===== CALENDÁRIO SIMPLES =====
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

  // Espaços vazios antes do primeiro dia
  for (let i = 0; i < firstDay; i++) {
    const empty = document.createElement("div");
    calendarDays.appendChild(empty);
  }

  for (let day = 1; day <= lastDate; day++) {
    const dateKey = `${year}-${month + 1}-${day}`;
    const dayDiv = document.createElement("div");
    dayDiv.classList.add("calendar-day");
    dayDiv.textContent = day;

    // Se tiver evento
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


//MOCKS BOLETINS
const alunos = [
  {
    nome: "Luccas Gonçalves",
    curso: "Engenharia de Software",
    materias: [
      { nome: "Algoritmos", media: 8.5, freq: "96%" },
      { nome: "Banco de Dados", media: 7.8, freq: "92%" },
      { nome: "Programação Web", media: 9.2, freq: "98%" },
    ],
  },
  {
    nome: "Marina Silva",
    curso: "Análise e Desenvolvimento de Sistemas",
    materias: [
      { nome: "Engenharia de Software", media: 8.2, freq: "94%" },
      { nome: "JavaScript Avançado", media: 9.0, freq: "97%" },
    ],
  },
];

document.getElementById("btnBuscarAluno").addEventListener("click", () => {
  const pesquisa = document.getElementById("searchAluno").value.toLowerCase();
  const aluno = alunos.find(a => a.nome.toLowerCase().includes(pesquisa));

  const resultadoBox = document.getElementById("resultadoAluno");
  const tabela = document.getElementById("tabelaMaterias");
  tabela.innerHTML = "";

  if (aluno) {
    document.getElementById("alunoNome").textContent = aluno.nome;
    document.getElementById("alunoCurso").textContent = aluno.curso;

    aluno.materias.forEach(m => {
      const row = `<tr>
        <td>${m.nome}</td>
        <td>${m.media}</td>
        <td>${m.freq}</td>
      </tr>`;
      tabela.innerHTML += row;
    });

    resultadoBox.classList.remove("hidden");

    // Atualiza o PDF atual com os dados do aluno
    window.alunoSelecionado = aluno;

  } else {
    resultadoBox.classList.add("hidden");
    alert("Aluno não encontrado!");
  }
});


// ===== GERAR BOLETIM (PDF) =====
function gerarBoletim() {
  const aluno = window.alunoSelecionado;

  // Se nenhum aluno foi selecionado ainda
  if (!aluno) {
    alert("Por favor, pesquise e selecione um aluno antes de gerar o boletim!");
    return;
  }

  const { jsPDF } = window.jspdf;
  const doc = new jsPDF("p", "mm", "a4");

  // ======= ESTILO BASE =======
  const corPrincipal = [255, 0, 84]; // Rosa TIVIT
  const corTexto = [40, 40, 40];
  const margem = 20;

  // ======= CABEÇALHO =======
  doc.setFillColor(...corPrincipal);
  doc.rect(0, 0, 210, 30, "F");

  doc.setFont("helvetica", "bold");
  doc.setFontSize(18);
  doc.setTextColor(255, 255, 255);
  doc.text("TIVIT Academy", margem, 20);

  doc.setFontSize(12);
  doc.text("Boletim Escolar", 200 - margem, 20, { align: "right" });

  // ======= DADOS DO ALUNO =======
  const nome = aluno.nome;
  const curso = aluno.curso;
  const semestre = "2º Semestre - 2025";
  const disciplinas = aluno.materias;

  doc.setTextColor(...corTexto);
  doc.setFontSize(12);
  doc.setFont("helvetica", "bold");
  doc.text("Nome do Aluno:", margem, 45);
  doc.setFont("helvetica", "normal");
  doc.text(nome, 60, 45);

  doc.setFont("helvetica", "bold");
  doc.text("Curso:", margem, 52);
  doc.setFont("helvetica", "normal");
  doc.text(curso, 35, 52);

  doc.setFont("helvetica", "bold");
  doc.text("Período:", margem, 59);
  doc.setFont("helvetica", "normal");
  doc.text(semestre, 45, 59);

  // ======= TABELA =======
  const inicioTabelaY = 75;
  const larguraTotal = 170;

  // Cabeçalho
  doc.setFillColor(245, 245, 245);
  doc.rect(margem, inicioTabelaY, larguraTotal, 10, "F");
  doc.setTextColor(...corPrincipal);
  doc.setFont("helvetica", "bold");

  doc.text("Disciplina", margem + 5, inicioTabelaY + 7);
  doc.text("Média Final", 105, inicioTabelaY + 7);
  doc.text("Frequência", 175, inicioTabelaY + 7, { align: "right" });

  // Linhas de conteúdo
  let y = inicioTabelaY + 17;
  doc.setTextColor(...corTexto);
  doc.setFont("helvetica", "normal");

  disciplinas.forEach((d) => {
    doc.text(d.nome, margem + 5, y);
    doc.text(String(d.media), 115, y);
    doc.text(String(d.freq), 170, y, { align: "right" });
    y += 9;
  });


  // ======= MÉDIA GERAL =======
  const mediaGeral =
    disciplinas.reduce((acc, d) => acc + parseFloat(d.media), 0) /
    disciplinas.length;
  doc.setFont("helvetica", "bold");
  doc.setTextColor(...corPrincipal);
  doc.text(
    `Média Geral: ${mediaGeral.toFixed(1)}`,
    200 - margem,
    y + 5,
    { align: "right" }
  );

  // ======= RODAPÉ =======
  doc.setDrawColor(...corPrincipal);
  doc.line(margem, y + 15, 190, y + 15);

  doc.setFont("helvetica", "italic");
  doc.setFontSize(10);
  doc.setTextColor(120, 120, 120);
  doc.text(
    "Documento gerado automaticamente pelo sistema TIVIT Academy",
    105,
    285,
    { align: "center" }
  );

  // ======= SALVAR =======
  const nomeArquivo = `Boletim_${nome.replace(/\s+/g, "_")}.pdf`;
  doc.save(nomeArquivo);
}


document.getElementById("btnBoletim").addEventListener("click", gerarBoletim);


// ===== CÁLCULO DE MÉDIAS E FREQUÊNCIA =====
const mediaForm = document.getElementById("mediaForm");
const resultado = document.getElementById("resultado");

mediaForm.addEventListener("submit", (e) => {
  e.preventDefault();
  const n1 = parseFloat(document.getElementById("nota1").value) || 0;
  const n2 = parseFloat(document.getElementById("nota2").value) || 0;
  const n3 = parseFloat(document.getElementById("nota3").value) || 0;
  const freq = parseFloat(document.getElementById("freq").value) || 0;

  const media = ((n1 + n2 + n3) / 3).toFixed(2);
  const status = media >= 7 && freq >= 75 ? "Aprovado" : "Reprovado";

  resultado.innerHTML = `
    Média: <strong>${media}</strong><br>
    Frequência: <strong>${freq}%</strong><br>
    Situação: <span style="color:${status.includes('Aprovado') ? 'lime' : 'red'}">${status}</span>
  `;
});
