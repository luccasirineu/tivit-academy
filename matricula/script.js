// === TEMA ===
const themeSwitch = document.getElementById("themeSwitch");
themeSwitch.addEventListener("change", () => {
  document.body.classList.toggle("light");
});

// === FORMATAÇÃO CPF ===
const cpfInput = document.getElementById("cpf");

cpfInput.addEventListener("input", () => {
  let value = cpfInput.value.replace(/\D/g, "");

  if (value.length > 3) value = value.replace(/(\d{3})(\d)/, "$1.$2");
  if (value.length > 7) value = value.replace(/(\d{3})(\d)/, "$1.$2");
  if (value.length > 11) value = value.replace(/(\d{3})(\d)/, "$1-$2");

  cpfInput.value = value;
});

// === ENVIO DO FORM ===
document.getElementById("formMatricula").addEventListener("submit", (e) => {
  e.preventDefault();
  alert("Matrícula realizada com sucesso! 🎉");
});

// === DESCRIÇÃO DOS CURSOS ===
const cursosDetalhes = {
  "Análise de Dados": 
    "Domine Python, SQL, visualização, dashboards e técnicas usadas por analistas de dados no mercado.",
  "Cibersegurança": 
    "Aprenda pentest, redes, criptografia, vulnerabilidades e segurança aplicada em ambientes reais.",
  "Desenvolvimento Web": 
    "Front-end, back-end, APIs, HTML, CSS, JavaScript e criação de sistemas completos.",
  "IA & Machine Learning": 
    "Modelos inteligentes, automação, predição, data pipelines e técnicas modernas de IA."
};

// === MOSTRAR MODAL ===
function showDetails(curso) {
  document.getElementById("modalTitle").innerText = curso;
  document.getElementById("modalText").innerText = cursosDetalhes[curso];
  document.getElementById("modal").style.display = "flex";
}

// === FECHAR MODAL ===
function closeModal() {
  document.getElementById("modal").style.display = "none";
}

// === ROLAR ATÉ O FORM ===
function goToForm() {
  document.getElementById("matricula").scrollIntoView({ behavior: "smooth" });
}

// === FECHAR MODAL AO CLICAR FORA ===
window.onclick = function (event) {
  const modal = document.getElementById("modal");
  if (event.target === modal) {
    modal.style.display = "none";
  }
};
