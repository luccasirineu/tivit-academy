import { NavLink, useNavigate } from "react-router-dom";
import logo from "../../assets/tivitLogo.png";
import { useAuth } from "../../context/AuthContext";

const navItems = [
  { to: "/admin/dashboard",      icon: "fas fa-home",        label: "Dashboard"      },
  { to: "/admin/solicitacoes",   icon: "fas fa-file",        label: "Solicitações"   },
  { to: "/admin/usuarios",       icon: "fas fa-list",        label: "Usuários"       },
  { to: "/admin/cursos",         icon: "fas fa-plus-square", label: "Cursos"         },
  { to: "/admin/turmas",         icon: "fas fa-users",       label: "Turmas"         },
  { to: "/admin/notificacoes",   icon: "fas fa-bell",        label: "Notificações"   },
  { to: "/admin/professores",    icon: "fas fa-clipboard",   label: "Professores"    },
  { to: "/admin/alunos",         icon: "fas fa-user-graduate", label: "Alunos" },
];

export default function SidebarAdmin({ collapsed, toggleSidebar, lightTheme, toggleTheme }: any) {
  const { logout } = useAuth();
  const navigate = useNavigate();

  return (
    <>
      <button className="sidebar-toggle-floating" onClick={toggleSidebar}>☰</button>
      <aside className={`sidebar ${collapsed ? "collapsed" : ""}`}>
        <div className="logo">
          <img src={logo} alt="TIVIT Academy" />
          <h2 className="logo-title">TIVIT ACADEMY</h2>
        </div>
        <nav style={{ marginTop: "65%" }}>
          <ul>
            {navItems.map(({ to, icon, label }) => (
              <li key={to}>
                <NavLink to={to} className={({ isActive }) => isActive ? "active" : ""}>
                  <i className={icon}></i> {label}
                </NavLink>
              </li>
            ))}
          </ul>
        </nav>
        <div className="sidebar-logout" onClick={() => { logout(); navigate("/login"); }}>
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
        <div className="toggle-btn" onClick={toggleSidebar}>☰</div>
      </aside>
    </>
  );
}