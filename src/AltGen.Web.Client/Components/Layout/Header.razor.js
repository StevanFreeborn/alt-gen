/**
 * @summary Shows a dialog element by calling the `showModal` method on it.
 * @param {HTMLDialogElement} dialog - The dialog element to show
 * @returns {void}
 */
export function showDialog(dialog) {
  dialog.showModal();
}

/**
 * @summary Closes a dialog element by calling the `close` method on it.
 * @param {HTMLDialogElement} dialog - The dialog element to close
 * @returns {void}
 */
export function closeDialog(dialog) {
  dialog.close();
}