// This script will add 50 items and 5 suppliers for testing via API calls.
// Run this in your browser console while logged in as admin.

const API_BASE_URL = 'http://192.168.10.125:5027/api';

async function addTestSuppliers() {
    const suppliers = [];
    for (let i = 1; i <= 5; i++) {
        suppliers.push({
            name: `Test Supplier ${i}`,
            afm: `AFM${1000 + i}`,
            phoneNumber: `21000000${i}`,
            address: `Test Address ${i}`,
            email: `supplier${i}@test.com`
        });
    }
    for (const s of suppliers) {
        await fetch(`${API_BASE_URL}/suppliers`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'X-Admin': 'true' },
            body: JSON.stringify(s)
        });
    }
    console.log('Test suppliers added.');
}

async function getSuppliers() {
    const res = await fetch(`${API_BASE_URL}/suppliers`);
    return await res.json();
}

async function addTestItems() {
    const suppliers = await getSuppliers();
    if (!suppliers.length) { console.error('No suppliers found!'); return; }
    for (let i = 1; i <= 50; i++) {
        const item = {
            name: `Test Item ${i}`,
            quantity: Math.floor(Math.random() * 100),
            location: `Shelf ${Math.ceil(i/10)}`,
            price: (Math.random() * 100).toFixed(2),
            supplierId: suppliers[i % suppliers.length].id
        };
        await fetch(`${API_BASE_URL}/storageitems`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'X-Admin': 'true' },
            body: JSON.stringify(item)
        });
    }
    console.log('Test items added.');
}

async function addTestData() {
    await addTestSuppliers();
    await addTestItems();
    alert('Test suppliers and items added! Refresh the page to see them.');
}

addTestData();
