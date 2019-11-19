function m = InsertRow(m, row, idx)
    if idx<=1
        m = [row; m];
    elseif idx>size(m,1)
        m = [m; row];
    else
        m = [m(1:idx-1,:); row; m(idx:end,:)];
    end
end