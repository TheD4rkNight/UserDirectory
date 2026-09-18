import { type FormEvent, useState } from 'react';
import { useMsal } from '@azure/msal-react';
import { useNavigate } from 'react-router-dom';
import RequireAuth from '../auth/RequireAuth';
import { createUser } from '../api/usersApi';
import { loginRequest } from '../auth/authConfig';
import Toast from '../components/Toast';

type FormState = { name: string; age: string; city: string; state: string; pincode: string };
type Errors = Partial<Record<keyof FormState, string>>;

const initialForm: FormState = { name: '', age: '', city: '', state: '', pincode: '' };

function AddUserForm() {
  const navigate = useNavigate();
  const { instance, accounts } = useMsal();
  const [form, setForm] = useState(initialForm);
  const [errors, setErrors] = useState<Errors>({});
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [toast, setToast] = useState('');

  const update = (field: keyof FormState, value: string) => setForm(prev => ({ ...prev, [field]: value }));

  const validate = (): Errors => {
    const next: Errors = {};
    const name = form.name.trim();
    if (!name) next.name = 'Name is required.';
    else if (name.length < 2 || name.length > 100) next.name = 'Name must be between 2 and 100 characters.';

    if (!form.age.trim()) next.age = 'Age is required.';
    else {
      const age = Number(form.age);
      if (!Number.isInteger(age) || age < 0 || age > 120) next.age = 'Age must be an integer between 0 and 120.';
    }
    if (!form.city.trim()) next.city = 'City is required.';
    if (!form.state.trim()) next.state = 'State is required.';
    const pincode = form.pincode.trim();
    if (!pincode) next.pincode = 'Pincode is required.';
    else if (pincode.length < 4 || pincode.length > 10) next.pincode = 'Pincode must be between 4 and 10 characters.';
    return next;
  };

  const submit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError('');
    const nextErrors = validate();
    setErrors(nextErrors);
    if (Object.keys(nextErrors).length > 0) return;

    try {
      setSaving(true);
      //const account = accounts[0];
      //if (!account) throw new Error('No signed-in account is available.');
      //const token = await instance.acquireTokenSilent({ ...loginRequest, account });
      
      await createUser({
        name: form.name.trim(),
        age: Number(form.age),
        city: form.city.trim(),
        state: form.state.trim(),
        pincode: form.pincode.trim(),
      });
      setToast('User created successfully.');
      window.setTimeout(() => navigate('/'), 600);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Unable to create user.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <section className="form-card">
      <h1>Add User</h1>
      <p>Create a new directory entry.</p>
      {toast && <Toast message={toast} />}
      {error && <div className="error" role="alert">{error}</div>}
      <form onSubmit={submit} noValidate>
        {(['name', 'age', 'city', 'state', 'pincode'] as const).map(field => (
          <div className="form-group" key={field}>
            <label htmlFor={field}>{field[0].toUpperCase() + field.slice(1)}</label>
            <input
              id={field}
              type={field === 'age' ? 'number' : 'text'}
              value={form[field]}
              onChange={event => update(field, event.target.value)}
              aria-invalid={Boolean(errors[field])}
              aria-describedby={errors[field] ? `${field}-error` : undefined}
            />
            {errors[field] && <span id={`${field}-error`} className="validation">{errors[field]}</span>}
          </div>
        ))}
        <button className="primary" type="submit" disabled={saving}>{saving ? 'Saving…' : 'Add User'}</button>
      </form>
    </section>
  );
}

export default function AddUserPage() {
  return (
    // <RequireAuth>
      <AddUserForm />
    // </RequireAuth>
  );
}
