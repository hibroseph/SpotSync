import { toast } from "react-toastify";

export default (message) => {
  toast(message);
};

export const error = (message) => {
  toast.error(message);
};
