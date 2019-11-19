function m = Mean(x)
    nanidx = isnan(x);
    x(nanidx) = [];
    m = mean(x);
end