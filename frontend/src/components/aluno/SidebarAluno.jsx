import { NavLink, useNavigate } from "react-router-dom";
import logo from "../../assets/tivitLogo.png";
import { useAuth } from "../../context/AuthContext";

export default function SidebarAluno({
  collapsed,
  toggleSidebar,
  lightTheme,
  toggleTheme,
}) {
  const { logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <>
      <button className="sidebar-toggle-floating" onClick={toggleSidebar}>
        ☰
      </button>
      <aside className={`sidebar ${collapsed ? "collapsed" : ""}`}>
        <div className="logo">
          <img src={logo} alt="TIVIT Academy" />
          <h2 className="logo-title">TIVIT ACADEMY</h2>
        </div>
        <nav style={{ marginTop: "65%" }}>
          <ul>
            <li>
              <NavLink to="/aluno/dashboard" className={({ isActive }) => isActive ? "active" : ""}>
                <i className="fas fa-home"></i> Dashboard
              </NavLink>
            </li>
            <li>
              <NavLink to="/aluno/calendario" className={({ isActive }) => isActive ? "active" : ""}>
                <i className="fas fa-calendar-alt"></i> Calendário
              </NavLink>
            </li>
            <li>
              <NavLink to="/aluno/desempenho" className={({ isActive }) => isActive ? "active" : ""}>
                <i className="fas fa-chart-line"></i> Desempenho
              </NavLink>
            </li>
            <li>
              <NavLink to="/aluno/materias" className={({ isActive }) => isActive ? "active" : ""}>
                <i className="fas fa-book"></i> Matérias
              </NavLink>
            </li>
            <li>
              <NavLink to="/aluno/relatorio" className={({ isActive }) => isActive ? "active" : ""}>
                <i className="fas fa-file-alt"></i> Relatório
              </NavLink>
            </li>

            <li>
              <NavLink to="/aluno/notificacoes" className={({ isActive }) => isActive ? "active" : ""}>
                <i className="fas fa-bell"></i> Notificações
              </NavLink>
            </li>
          </ul>
        </nav>

        <div className="sidebar-logout" onClick={handleLogout}>
          <i className="fas fa-sign-out-alt"></i>
          <span className="logout-text">Sair</span>
        </div>

        <div className="theme-toggle">
          <span>{lightTheme ? "Modo Claro" : "Modo Escuro"}</span>
          <label className="switch">
            <input type="checkbox" checked={lightTheme} onChange={toggleTheme} />
            <span className="slider"></span>
          </label>
        </div>

        

        <div className="toggle-btn" onClick={toggleSidebar}>
          ☰
        </div>
      </aside>
    </>
  );
}