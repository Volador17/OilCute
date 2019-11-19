function m = InsertColumn(m, column, idx)
    if idx <= 1
        m = [column m]; 
    elseif idx > size(m,2)
        m = [m column];
    else
        m = [m(:,1:idx-1)  column  m(:,idx:end)];
    end
end