import React, { useState, useEffect } from 'react';
import { fetchCourses } from '../../services/curso.service';
import { startEnrollment, uploadPayment, uploadDocuments } from '../../services/matricula.service';
import { Modal } from '../common/Modal';
import type { Course } from '../../types/index';

export function EnrollmentForm() {
  const [currentStep, setCurrentStep] = useState(1);
  const [courses, setCourses] = useState<Course[]>([]);
  
  // Form data state
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [cpf, setCpf] = useState('');
  const [cursoId, setCursoId] = useState<number | null>(null);
  const [comprovante, setComprovante] = useState<File | null>(null);
  const [docHistorico, setDocHistorico] = useState<File | null>(null);
  const [docCpf, setDocCpf] = useState<File | null>(null);

  // API state
  const [matriculaId, setMatriculaId] = useState<number | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Modal state
  const [isSuccessOpen, setIsSuccessOpen] = useState(false);
  const [isErrorOpen, setIsErrorOpen] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    // Fetch courses to populate the select dropdown
    const loadCourses = async () => {
      try {
        const data = await fetchCourses();
        setCourses(data);
      } catch (err) {
        handleApiError(err, 'Não foi possível carregar os cursos.');
      }
    };
    loadCourses();
  }, []);

  const handleApiError = (err: unknown, defaultMessage: string) => {
    const message = err instanceof Error ? err.message : defaultMessage;
    setErrorMessage(message);
    setIsErrorOpen(true);
  };
  
  const resetForm = () => {
    setCurrentStep(1);
    setNome('');
    setEmail('');
    setCpf('');
    setCursoId(null);
    setComprovante(null);
    setDocHistorico(null);
    setDocCpf(null);
    setMatriculaId(null);
  };

  const handleNextStep = async () => {
    setIsSubmitting(true);
    try {
      if (currentStep === 1) {
        if (!nome || !email || !cpf || !cursoId) {
          throw new Error("Por favor, preencha todos os dados pessoais.");
        }
        const response = await startEnrollment({ nome, email, cpf, cursoId });
        setMatriculaId(response.matriculaId);
        setCurrentStep(2);
      } else if (currentStep === 2) {
        if (!matriculaId || !comprovante) {
          throw new Error("Por favor, envie o comprovante de pagamento.");
        }
        await uploadPayment(matriculaId, comprovante);
        setCurrentStep(3);
      }
    } catch (err) {
      handleApiError(err, 'Ocorreu um erro.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handlePrevStep = () => {
    if (currentStep > 1) {
      setCurrentStep(currentStep - 1);
    }
  };

  const handleFinalSubmit = async () => {
    setIsSubmitting(true);
    try {
      if (!matriculaId || !docHistorico || !docCpf) {
        throw new Error("Por favor, envie todos os documentos finais.");
      }
      await uploadDocuments(matriculaId, docHistorico, docCpf);
      setIsSuccessOpen(true);
      resetForm();
    } catch (err) {
      handleApiError(err, 'Ocorreu um erro ao finalizar a matrícula.');
    } finally {
      setIsSubmitting(false);
    }
  };
  
  const handleCpfChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    let value = e.target.value.replace(/\D/g, "");
    if (value.length > 11) value = value.slice(0, 11); // Ensure CPF is not longer than 11 digits
    value = value.replace(/(\d{3})(\d)/, "$1.$2");
    value = value.replace(/(\d{3})(\d)/, "$1.$2");
    value = value.replace(/(\d{3})(\d{1,2})$/, "$1-$2");
    setCpf(value);
  };

  return (
    <>
      <section id="matricula" className="form-section">
        <h2>Matrícula</h2>
        <div className="wizard-container">
          <div className="progress-bar">
            <div className={`step ${currentStep >= 1 ? 'active' : ''}`}>1</div>
            <div className={`step ${currentStep >= 2 ? 'active' : ''}`}>2</div>
            <div className={`step ${currentStep >= 3 ? 'active' : ''}`}>3</div>
          </div>

          {/* Step 1 */}
          <div className="wizard-step" style={{ display: currentStep === 1 ? 'block' : 'none' }}>
            <h2>Dados Pessoais</h2>
            <label>Nome Completo</label>
            <input type="text" value={nome} onChange={(e) => setNome(e.target.value)} required />
            <label>Email</label>
            <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
            <label>CPF</label>
            <input type="text" value={cpf} onChange={handleCpfChange} required />
            <label>Curso</label>
            <select value={cursoId ?? ''} onChange={(e) => setCursoId(Number(e.target.value))} required>
              <option value="" disabled>Selecione um curso</option>
              {courses.map(course => <option key={course.id} value={course.id}>{course.nome}</option>)}
            </select>
            <div className="wizard-buttons">
              <button className="next" onClick={handleNextStep} disabled={isSubmitting}>
                {isSubmitting ? 'Enviando...' : 'Próximo'}
              </button>
            </div>
          </div>

          {/* Step 2 */}
          <div className="wizard-step" style={{ display: currentStep === 2 ? 'block' : 'none' }}>
            <h2>Comprovante de Pagamento</h2>
            <p>Envie um PDF ou imagem.</p>
            <input type="file" accept="application/pdf,image/*" onChange={(e) => setComprovante(e.target.files ? e.target.files[0] : null)} required />
            <div className="wizard-buttons">
              <button className="back" onClick={handlePrevStep} disabled={isSubmitting}>Voltar</button>
              <button className="next" onClick={handleNextStep} disabled={isSubmitting}>
                {isSubmitting ? 'Enviando...' : 'Próximo'}
              </button>
            </div>
          </div>

          {/* Step 3 */}
          <div className="wizard-step" style={{ display: currentStep === 3 ? 'block' : 'none' }}>
            <h2>Documentos Finais</h2>
            <p>Envie Histórico escolar.</p>
            <input type="file" accept="application/pdf,image/*" onChange={(e) => setDocHistorico(e.target.files ? e.target.files[0] : null)} required />
            <p>Envie CPF (RG, CNH).</p>
            <input type="file" accept="application/pdf,image/*" onChange={(e) => setDocCpf(e.target.files ? e.target.files[0] : null)} required />
            <div className="wizard-buttons">
              <button className="back" onClick={handlePrevStep} disabled={isSubmitting}>Voltar</button>
              <button className="finish" onClick={handleFinalSubmit} disabled={isSubmitting}>
                {isSubmitting ? 'Finalizando...' : 'Finalizar'}
              </button>
            </div>
          </div>
        </div>
      </section>

      {/* Modals */}
      <Modal isOpen={isSuccessOpen} onClose={() => setIsSuccessOpen(false)} title="Matrícula concluída!">
        <p>Sua matrícula foi enviada com sucesso. Aguarde as informações no seu e-mail.</p>
        <button className="btn-modal" onClick={() => setIsSuccessOpen(false)}>Fechar</button>
      </Modal>

      <Modal isOpen={isErrorOpen} onClose={() => setIsErrorOpen(false)} title="Erro" isError>
        <p>{errorMessage}</p>
        <button className="btn-fechar" onClick={() => setIsErrorOpen(false)}>Fechar</button>
      </Modal>
    </>
  );
}
