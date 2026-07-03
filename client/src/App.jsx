import { useMemo, useState } from 'react'
import initialEmployees from './data/employees.json'

const emptyForm = { name: '', email: '', department: '', salary: '' }

export default function App() {
  const [allEmployees, setAllEmployees] = useState(initialEmployees)
  const [search, setSearch] = useState('')
  const [department, setDepartment] = useState('')
  const [form, setForm] = useState(emptyForm)
  const [error, setError] = useState('')

  const employees = allEmployees.filter((e) => e.isActive)

  const departments = useMemo(
    () => [...new Set(employees.map((e) => e.department))].sort(),
    [employees],
  )

  const filtered = employees.filter((e) => {
    const matchesName = e.name.toLowerCase().includes(search.toLowerCase())
    const matchesDept = !department || e.department === department
    return matchesName && matchesDept
  })

  const deactivate = (id) => {
    setAllEmployees((prev) =>
      prev.map((e) => (e.id === id ? { ...e, isActive: false } : e)),
    )
  }

  const submit = (event) => {
    event.preventDefault()
    setError('')

    if (!form.name.trim() || !form.email.trim() || !form.department.trim()) {
      setError('Name, email, and department are required.')
      return
    }
    if (allEmployees.some((e) => e.email.toLowerCase() === form.email.trim().toLowerCase())) {
      setError('Email already exists.')
      return
    }

    const employee = {
      id: allEmployees.length ? Math.max(...allEmployees.map((e) => e.id)) + 1 : 1,
      name: form.name.trim(),
      email: form.email.trim(),
      department: form.department.trim(),
      salary: Number(form.salary) || 0,
      isActive: true,
    }
    setAllEmployees((prev) => [...prev, employee])
    setForm(emptyForm)
  }

  return (
    <div style={{ fontFamily: 'sans-serif', padding: 24, maxWidth: 900 }}>
      <h1>Employees</h1>

      <div style={{ display: 'flex', gap: 12, marginBottom: 16 }}>
        <input
          placeholder="Search by name"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <select value={department} onChange={(e) => setDepartment(e.target.value)}>
          <option value="">All departments</option>
          {departments.map((d) => (
            <option key={d} value={d}>{d}</option>
          ))}
        </select>
      </div>

      <table border="1" cellPadding="8" style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr>
            <th>Name</th>
            <th>Email</th>
            <th>Dept</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {filtered.map((e) => (
            <tr key={e.id}>
              <td>{e.name}</td>
              <td>{e.email}</td>
              <td>{e.department}</td>
              <td>
                <button onClick={() => deactivate(e.id)}>Deactivate</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <h2 style={{ marginTop: 32 }}>Add Employee</h2>
      {error && <p style={{ color: 'crimson' }}>{error}</p>}
      <form onSubmit={submit} style={{ display: 'grid', gap: 8, maxWidth: 360 }}>
        <input required placeholder="Name" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
        <input required type="email" placeholder="Email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
        <input required placeholder="Department" value={form.department} onChange={(e) => setForm({ ...form, department: e.target.value })} />
        <input required type="number" placeholder="Salary" value={form.salary} onChange={(e) => setForm({ ...form, salary: e.target.value })} />
        <button type="submit">Create</button>
      </form>
    </div>
  )
}
