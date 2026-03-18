import { NavLink, useNavigate } from "react-router-dom";
import logo from "../../assets/tivitLogo.png";
import { useAuth } from "../../context/AuthContext";

export default function SidebarProfessor({
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
              <NavLink to="/professor/dashboardProfessor" className={({ isActive }) => isActive ? "active" : ""}>
                <i className="fas fa-home"></i> Dashboard
              </NavLink>
            </li>
            <li>
              <NavLink to="/professor/calendario" className={({ isActive }) => isActive ? "active" : ""}>
                <i className="bx bx-calendar"></i> Calendário
              </NavLink>
            </li>
            <li>
              <NavLink to="/professor/boletim" className={({ isActive }) => isActive ? "active" : ""}>
                <i className="bx bx-file"></i> Boletim
              </NavLink>
            </li>
            <li>
              <NavLink to="/professor/chamada" className={({ isActive }) => isActive ? "active" : ""}>
                <i className="bx bx-check-square"></i> Chamada
              </NavLink>
            </li>
            <li>
              <NavLink to="/professor/conteudo" className={({ isActive }) => isActive ? "active" : ""}>
                <i className="bx  bx-book-content"></i> Conteudo
              </NavLink>
            </li>

            <li>
              <NavLink to="/professor/notas" className={({ isActive }) => isActive ? "active" : ""}>
                <i className="bx bx-calculator"></i> Notas
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