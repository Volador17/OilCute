function yp = annp(x, w1,b1,w2,b2,f1,f2)

x = x';

dim = length(size(w1));

if dim>2
    for j=1:size(w1,1)
        w10(:,:)=w1(j,:,:);
        b10=b1(:,j);
        w20=w2(j,:);
        b20=b2(j);
        yreg(j,:) = simuff(x,w10,b10,f1,w20,b20,f2);
    end
    yp=mean(yreg)' ;
else
    yp = simuff(x,w1,b1,f1,w2,b2,f2) ;
    yp = yp';
end



end

