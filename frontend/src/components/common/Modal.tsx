import React from 'react';

interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title?: string;
  children: React.ReactNode;
  isError?: boolean; // To distinguish error modal styles
}

export function Modal({ isOpen, onClose, title, children, isError = false }: ModalProps) {
  if (!isOpen) {
    return null;
  }

  // Base classes for the modal
  const modalClass = isError ? 'modal-erro' : 'modal';
  const contentClass = isError ? 'modal-conteudo' : 'modal-content';
  const closeButtonClass = isError ? 'modal-fechar' : 'close';

  return (
    <div id="modal" className={modalClass} style={{ display: 'flex' }} onClick={onClose}>
      <div className={contentClass} onClick={(e) => e.stopPropagation()}>
        <span className={closeButtonClass} onClick={onClose}>&times;</span>
        {title && <h2 id="modalTitle">{title}</h2>}
        {children}
      </div>
    </div>
  );
}
