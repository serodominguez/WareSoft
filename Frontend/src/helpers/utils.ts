// Normalizar texto: quitar tildes y convertir a minúsculas
export const normalize = (text: string): string => {
  return text
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, ''); // Elimina tildes
};